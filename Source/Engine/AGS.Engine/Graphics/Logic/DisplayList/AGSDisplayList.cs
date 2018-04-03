using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using AGS.API;

namespace AGS.Engine
{
    public class AGSDisplayList : IDisplayList
    {
        private readonly IGameState _gameState;
        private readonly IInput _input;
        private readonly IImageRenderer _renderer;
        private readonly IComparer<IObject> _comparer;
        private readonly List<IObject> _emptyList = new List<IObject>(1);
        private readonly HashSet<string> _alreadyPrepared = new HashSet<string>();
        private readonly IAGSRoomTransitions _roomTransitions;
        private readonly IMatrixUpdater _matrixUpdater;

        private readonly ConcurrentDictionary<IViewport, List<IObject>> _cache;
        private readonly ConcurrentDictionary<IViewport, ViewportSubscriber> _viewportSubscribers;
        private readonly ConcurrentDictionary<string, List<API.IComponentBinding>> _bindings;

        private IRoom _lastRoom;
        private IObject _lastRoomBackground;
        private bool _isDirty;

        private struct ViewportSubscriber
        {
            private IViewport _viewport;
            private Action _onSomethingChanged;

            public ViewportSubscriber(IViewport viewport, Action onSomethingChanged)
            {
                _viewport = viewport;
                _onSomethingChanged = onSomethingChanged;
                viewport.DisplayListSettings.PropertyChanged += onDisplayListPropertyChanged;
                viewport.DisplayListSettings.RestrictionList.PropertyChanged += onDisplayListPropertyChanged;
                viewport.DisplayListSettings.RestrictionList.RestrictionList.OnListChanged.Subscribe(onRestrictionListChanged);
                viewport.DisplayListSettings.DepthClipping.PropertyChanged += onDisplayListPropertyChanged;
            }

            private void onRestrictionListChanged(AGSHashSetChangedEventArgs<string> obj)
            {
                _onSomethingChanged();
            }

            private void onDisplayListPropertyChanged(object sender, PropertyChangedEventArgs args)
            {
                _onSomethingChanged();
            }
        }

        private class AnimationSubscriber
        {
            private IAnimation _lastAnimation;
            private ISprite _lastSprite;
            private float _lastX, _lastZ;
            private IObject _obj;
            private Action _onSomethingChanged;

            private class BindingWrapper : API.IComponentBinding
            {
                private API.IComponentBinding _binding;
                private Action _unsubscribe;

                public BindingWrapper(API.IComponentBinding binding, Action unsubscribe)
                {
                    _binding = binding;
                    _unsubscribe = unsubscribe;
                }

                public void Unbind()
                {
                    _binding?.Unbind();
                    _unsubscribe();
                }
            }

            public AnimationSubscriber(IObject obj, Action onSomethingChanged)
            {
                _obj = obj;
                _onSomethingChanged = onSomethingChanged;
                _lastAnimation = null;
                _lastSprite = null;
                _lastX = float.MinValue;
                _lastZ = float.MinValue;
            }

            public API.IComponentBinding Bind()
            {
                subscribeAnimation();
                return new BindingWrapper(bind<IAnimationComponent>(_obj, onObjAnimationPropertyChanged), unsubscribeAll);
            }

            private void unsubscribeAll()
            {
                unsubscribeLastAnimation();
                unsubscribeLastSprite();
            }

            private void onObjAnimationPropertyChanged(object sender, PropertyChangedEventArgs args)
            {
                if (args.PropertyName != nameof(IAnimationComponent.Animation)) return;
                subscribeAnimation();
            }

            private void subscribeAnimation()
            {
                unsubscribeLastAnimation();
                _lastAnimation = _obj.Animation;
                var state = _obj.Animation?.State;
                if (state != null)
                {
                    state.PropertyChanged += onAnimationStatePropertyChanged;
                }
                subscribeSprite();
            }

            private void unsubscribeLastAnimation()
            {
                var state = _lastAnimation?.State;
                if (state != null)
                {
                    state.PropertyChanged -= onAnimationStatePropertyChanged;
                }
            }

            private void onAnimationStatePropertyChanged(object sender, PropertyChangedEventArgs args)
            {
                if (args.PropertyName != nameof(IAnimationState.CurrentFrame)) return;
                subscribeSprite();
            }

            private void onSpritePropertyChanged(object sender, PropertyChangedEventArgs args)
            {
                if (args.PropertyName == nameof(ISprite.Z)) onSpriteZChange();
                else if (args.PropertyName == nameof(ISprite.X)) onSpriteXChange();
            }

            private void onSpriteZChange()
            {
                var sprite = _obj.Animation?.Sprite;
                if (sprite == null) return;
                if (MathUtils.FloatEquals(_lastZ, sprite.Z)) return;
                _lastZ = sprite.Z;
                _onSomethingChanged();
            }

            private void onSpriteXChange()
            {
                var sprite = _obj.Animation?.Sprite;
                if (sprite == null) return;
                if (MathUtils.FloatEquals(_lastX, sprite.X)) return;
                _lastX = sprite.X;
                _onSomethingChanged();
            }

            private void subscribeSprite()
            {
                unsubscribeLastSprite();
                var newSprite = _obj.Animation?.Sprite;
                if (newSprite != null) newSprite.PropertyChanged += onSpritePropertyChanged;
                _lastSprite = newSprite;
                onSpriteZChange();
                onSpriteXChange();
            }

            private void unsubscribeLastSprite()
            {
                var lastSprite = _lastSprite;
                if (lastSprite != null) lastSprite.PropertyChanged -= onSpritePropertyChanged;
            }
        }

        public AGSDisplayList(IGameState gameState, IInput input, 
                              IImageRenderer renderer, IMatrixUpdater matrixUpdater, IAGSRoomTransitions roomTransitions)
        {
            _matrixUpdater = matrixUpdater;
            _roomTransitions = roomTransitions;
            _gameState = gameState;
            _input = input;
            _renderer = renderer;
            _cache = new ConcurrentDictionary<IViewport, List<IObject>>();
            _viewportSubscribers = new ConcurrentDictionary<IViewport, ViewportSubscriber>();
            _bindings = new ConcurrentDictionary<string, List<API.IComponentBinding>>();
            _comparer = new RenderOrderSelector();

            gameState.UI.OnListChanged.Subscribe(onUiChanged);
            subscribeObjects(gameState.UI);
            subscribeRoom();
            onSomethingChanged();
        }

        public List<IObject> GetDisplayList(IViewport viewport)
        {
            _viewportSubscribers.GetOrAdd(viewport, v => new ViewportSubscriber(v, onSomethingChanged));
            if (_gameState.Room != _lastRoom)
            {
                unsubscribeLastRoom();
                subscribeRoom();
                onSomethingChanged();
            }
            _cache.TryGetValue(viewport, out var displayList);
            return displayList ?? _emptyList;
		}

        public void Update()
        {
            _alreadyPrepared.Clear();
            bool isDirty = _isDirty || _roomTransitions.State == RoomTransitionState.PreparingNewRoomRendering;
            _isDirty = false;

            _matrixUpdater.ClearCache();
            foreach (var viewport in _viewportSubscribers.Keys)
            {
                List<IObject> list;
                if (isDirty)
                {
                    list = getDisplayList(viewport);
                    _cache[viewport] = list;
                }
                else
                {
                    list = _cache.GetOrAdd(viewport, getDisplayList);
                }
                foreach (var item in list)
                {
                    if (_alreadyPrepared.Add(item.ID))
                    {
                        _matrixUpdater.RefreshMatrix(item);
                    }
                }
            }
        }

        private List<IObject> getDisplayList(IViewport viewport)
        {
            var settings = viewport.DisplayListSettings;
            var room = viewport.RoomProvider.Room;
            int count = 1 + (room == null ? 0 : room.Objects.Count) + _gameState.UI.Count;

            var displayList = new List<IObject>(count);

            if (settings.DisplayRoom && room != null)
            {
                if (room.Background != null)
                    addToDisplayList(displayList, room.Background, viewport);

                foreach (IObject obj in room.Objects)
                {
                    if (!room.ShowPlayer && obj == _gameState.Player)
                        continue;
                    addToDisplayList(displayList, obj, viewport);
                }

                foreach (var area in room.Areas) addDebugDrawArea(displayList, area, viewport);
            }

            if (settings.DisplayGUIs)
            {
                foreach (IObject ui in _gameState.UI)
                {
                    addToDisplayList(displayList, ui, viewport);
                }
            }

            displayList.Sort(_comparer);
            return displayList;
        }

        private void addDebugDrawArea(List<IObject> displayList, IArea area, IViewport viewport)
		{
			if (area.Mask.DebugDraw == null) return;
			addToDisplayList(displayList, area.Mask.DebugDraw, viewport);
		}

        private void addToDisplayList(List<IObject> displayList, IObject obj, IViewport viewport)
		{
            if (!viewport.IsObjectVisible(obj))
			{
                return;
			}

            displayList.Add(obj);
		}

        private void onUiChanged(AGSHashSetChangedEventArgs<IObject> args)
        {
            if (args.ChangeType == ListChangeType.Add) subscribeObjects(args.Items);
            else unsubscribeObjects(args.Items);
            onSomethingChanged();
        }

        private void onRoomObjectsChanged(AGSHashSetChangedEventArgs<IObject> args)
        {
            if (args.ChangeType == ListChangeType.Add) subscribeObjects(args.Items);
            else unsubscribeObjects(args.Items);
            onSomethingChanged();
        }

        private void subscribeObjects(IEnumerable<IObject> items)
        {
            foreach (var item in items) subscribeObj(item);
        }

        private void unsubscribeObjects(IEnumerable<IObject> items)
        {
            foreach (var item in items) unsubscribeObj(item);
        }

        private void onAreaListChanged(AGSListChangedEventArgs<IArea> args)
        {
            if (args.ChangeType == ListChangeType.Add) subscribeAreas(args.Items.Select(a => a.Item));
            else unsubscribeAreas(args.Items.Select(a => a.Item));

            onSomethingChanged();
        }

        private void subscribeAreas(IAGSBindingList<IArea> areas)
        {
            areas.OnListChanged.Subscribe(onAreaListChanged);
            subscribeAreas((IEnumerable<IArea>)areas);
        }

        private void unsubscribeAreas(IAGSBindingList<IArea> areas)
        {
            areas.OnListChanged.Unsubscribe(onAreaListChanged);
            unsubscribeAreas((IEnumerable<IArea>)areas);
        }

        private void subscribeAreas(IEnumerable<IArea> areas)
        {
            foreach (var area in areas) subscribeArea(area);
        }

        private void unsubscribeAreas(IEnumerable<IArea> areas)
        {
            foreach (var area in areas) unsubscribeArea(area);
        }

        private void onSomethingChanged()
        {
            _isDirty = true;
        }

        private void onRoomPropertyChanged(object sender, PropertyChangedEventArgs args)
        {
            if (args.PropertyName != nameof(IRoom.Background) && args.PropertyName != nameof(IRoom.RoomLimitsProvider)
                && args.PropertyName != nameof(IRoom.ShowPlayer)) return;
            onSomethingChanged();
            if (args.PropertyName == nameof(IRoom.Background))
            {
                unsubscribeObj(_lastRoomBackground);
                _lastRoomBackground = _lastRoom.Background;
                subscribeObj(_lastRoom.Background);
            }
        }

        private void onObjVisibleChanged(object sender, PropertyChangedEventArgs args)
        {
            if (args.PropertyName != nameof(IVisibleComponent.Visible)) return;
            onSomethingChanged();
        }

        private void onObjTranslatePropertyChanged(object sender, PropertyChangedEventArgs args)
        {
            if (args.PropertyName != nameof(ITranslateComponent.Z) &&
                args.PropertyName != nameof(ITranslateComponent.X)) return;
            onSomethingChanged();
        }

        private void onObjDrawablePropertyChanged(object sender, PropertyChangedEventArgs args)
        {
            if (args.PropertyName != nameof(IDrawableInfoComponent.RenderLayer)) return;
            onSomethingChanged();
        }

        private void onAreaPropertyChanged(object sender, PropertyChangedEventArgs args)
        {
            onSomethingChanged();
        }

        private void onWalkBehindPropertyChanged(object sender, PropertyChangedEventArgs args)
        {
            onSomethingChanged();
        }

        private void subscribeRoom()
        {
            unsubscribeLastRoom();
            var room = _gameState.Room;
            _lastRoom = room;
            if (room == null)
            {
                _lastRoomBackground = null;
                return;
            }
            _lastRoomBackground = room.Background;

            room.Objects.OnListChanged.Subscribe(onRoomObjectsChanged);
            room.PropertyChanged += onRoomPropertyChanged;
            subscribeObj(room.Background);
            subscribeObjects(room.Objects);
            subscribeAreas(room.Areas);
        }

        private void unsubscribeLastRoom()
        {
            var lastRoom = _lastRoom;
            if (lastRoom == null) return;

            lastRoom.Objects.OnListChanged.Unsubscribe(onRoomObjectsChanged);
            lastRoom.PropertyChanged -= onRoomPropertyChanged;
            unsubscribeObj(lastRoom.Background);
            unsubscribeObjects(lastRoom.Objects);
            unsubscribeAreas(lastRoom.Areas);
        }

        private static API.IComponentBinding bind<TComponent>(IEntity entity, PropertyChangedEventHandler ev) where TComponent : API.IComponent
        {
            return entity.Bind<TComponent>(c => c.PropertyChanged += ev, c => c.PropertyChanged -= ev);
        }

        private void subscribeObj(IObject obj)
        {
            if (obj == null) return;
            var vBinding = bind<IVisibleComponent>(obj, onObjVisibleChanged);
            var tBinding = bind<ITranslateComponent>(obj, onObjTranslatePropertyChanged);
            var dBinding = bind<IDrawableInfoComponent>(obj, onObjDrawablePropertyChanged);

            AnimationSubscriber animSubscriber = new AnimationSubscriber(obj, onSomethingChanged);
            var aBinding = animSubscriber.Bind();

            obj.TreeNode.OnParentChanged.Subscribe(onSomethingChanged);

            _bindings[obj.ID] = new List<API.IComponentBinding> { vBinding, tBinding, dBinding, aBinding };
        }

        private void unsubscribeObj(IObject obj)
        {
            if (obj == null) return;
            obj.TreeNode.OnParentChanged.Unsubscribe(onSomethingChanged);
            unsubscribeEntity(obj);
        }

        private void unsubscribeEntity(IEntity entity)
        {
            _bindings.TryRemove(entity.ID, out var bindings);
            if (bindings == null) return;
            foreach (var binding in bindings) binding?.Unbind();
        }

        private void subscribeArea(IArea area)
        {
            if (area == null) return;
            subscribeObj(area.Mask?.DebugDraw);
            var aBinding = bind<IAreaComponent>(area, onAreaPropertyChanged);
            var bBinding = bind<IWalkBehindArea>(area, onWalkBehindPropertyChanged);
            _bindings[area.ID] = new List<API.IComponentBinding> { aBinding, bBinding };
        }

        private void unsubscribeArea(IArea area)
        {
            if (area == null) return;
            unsubscribeObj(area.Mask?.DebugDraw);
            unsubscribeEntity(area);
        }
    }
}