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
        private readonly IComparer<IObject> _comparer;
        private readonly List<IObject> _emptyList = new List<IObject>(1);
        private readonly HashSet<string> _alreadyPrepared = new HashSet<string>();
        private readonly IAGSRoomTransitions _roomTransitions;
        private readonly IMatrixUpdater _matrixUpdater;

        private class LayerListComparer : IComparer<(int z, List<IObject>)>
        {
            public int Compare((int z, List<IObject>) x, (int z, List<IObject>) y)
            {
                return y.z - x.z;
            }
        }

        private class LayerDisplayList
        {
            private IComparer<IObject> _comparer;
            private bool _isDirty;

            public LayerDisplayList(IComparer<IObject> comparer)
            {
                _comparer = comparer;
                IsDirty = true;
            }

            public bool IsDirty 
            {
                get => _isDirty;
                set
                {
                    _isDirty = value;
                    if (value) Items = new List<IObject>(100);
                }
            }
            public List<IObject> Items { get; private set; }

            public void Sort()
            {
                if (!IsDirty) return;
                Items.Sort(_comparer);
                IsDirty = false;
            }
        }

        private class ViewportDisplayList
        {
            private LayerListComparer _comparer;

            public ViewportDisplayList()
            {
                _comparer = new LayerListComparer();
                DisplayListPerLayer = new ConcurrentDictionary<int, LayerDisplayList>();
            }

            public List<IObject> DisplayList { get; private set; }
            public ConcurrentDictionary<int, LayerDisplayList> DisplayListPerLayer { get; private set; }

            public void Sort()
            {
                List<(int z, List<IObject> items)> layers = new List<(int, List<IObject>)>();
                int count = 0;
                foreach (var pair in DisplayListPerLayer)
                {
                    count += pair.Value.Items.Count;
                    pair.Value.Sort();
                    layers.Add((pair.Key, pair.Value.Items));
                }
                layers.Sort(_comparer);
                var displayList = new List<IObject>(count);
                foreach (var layer in layers)
                {
                    displayList.AddRange(layer.items);
                }
                DisplayList = displayList;
            }
        }

        private readonly ConcurrentDictionary<IViewport, ViewportDisplayList> _cache;
        private readonly ConcurrentDictionary<IViewport, ViewportSubscriber> _viewportSubscribers;
        private readonly ConcurrentDictionary<string, EntitySubscriber> _entitySubscribers;

        private IRoom _lastRoom;
        private IObject _lastRoomBackground;
        private bool _isDirty;
        private IObject _cursor;

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
            private IAnimationComponent _animationComponent;
            private IComponentBinding _animationComponentBinding;
            private IAnimation _lastAnimation;
            private ISprite _lastSprite;
            private float _lastX, _lastZ;
            private IEntity _obj;
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

            public AnimationSubscriber(IEntity obj, Action onSomethingChanged)
            {
                _animationComponentBinding = obj.Bind<IAnimationComponent>(c => _animationComponent = c, _ => _animationComponent = null);
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
                _animationComponentBinding?.Unbind();
            }

            private void onObjAnimationPropertyChanged(object sender, PropertyChangedEventArgs args)
            {
                if (args.PropertyName != nameof(IAnimationComponent.Animation)) return;
                subscribeAnimation();
            }

            private void subscribeAnimation()
            {
                unsubscribeLastAnimation();
                _lastAnimation = _animationComponent?.Animation;
                var state = _animationComponent?.Animation?.State;
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
                var sprite = _animationComponent?.Animation?.Sprite;
                if (sprite == null) return;
                if (MathUtils.FloatEquals(_lastZ, sprite.Z)) return;
                _lastZ = sprite.Z;
                _onSomethingChanged();
            }

            private void onSpriteXChange()
            {
                var sprite = _animationComponent?.Animation?.Sprite;
                if (sprite == null) return;
                if (MathUtils.FloatEquals(_lastX, sprite.X)) return;
                _lastX = sprite.X;
                _onSomethingChanged();
            }

            private void subscribeSprite()
            {
                unsubscribeLastSprite();
                var newSprite = _animationComponent?.Animation?.Sprite;
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

        private class EntitySubscriber
        {
            private IEntity _entity;
            private Action<int> _onLayerChanged;
            private int _layer;
            private List<IComponentBinding> _bindings;
            private IInObjectTreeComponent _tree;

            public EntitySubscriber(IEntity entity, Action<int> onLayerChanged)
            {
                _entity = entity;
                _onLayerChanged = onLayerChanged;
                entity.Bind<IDrawableInfoComponent>(c => _layer = c.RenderLayer?.Z ?? 0, _ => _layer = 0);

                var vBinding = bind<IVisibleComponent>(entity, onObjVisibleChanged);
                var tBinding = bind<ITranslateComponent>(entity, onObjTranslatePropertyChanged);
                var dBinding = bind<IDrawableInfoComponent>(entity, onObjDrawablePropertyChanged);

                AnimationSubscriber animSubscriber = new AnimationSubscriber(entity, onSomethingChanged);
                var aBinding = animSubscriber.Bind();

                var trBinding = entity.Bind<IInObjectTreeComponent>(c => { _tree = c; c.TreeNode.OnParentChanged.Subscribe(onSomethingChanged); onSomethingChanged(); }, c => { _tree = null; c.TreeNode.OnParentChanged.Unsubscribe(onSomethingChanged); onSomethingChanged(); });

                _bindings = new List<API.IComponentBinding> { vBinding, tBinding, dBinding, aBinding, trBinding };
            }

            public void Unsubscribe()
            {
                _tree?.TreeNode.OnParentChanged.Unsubscribe(onSomethingChanged);
                foreach (var binding in _bindings) binding?.Unbind();
            }

            private void onObjVisibleChanged(object sender, PropertyChangedEventArgs args)
            {
                if (args.PropertyName != nameof(IVisibleComponent.Visible)) return;
                onSomethingChanged();
            }

            private void onObjTranslatePropertyChanged(object sender, PropertyChangedEventArgs args)
            {
                if (args.PropertyName != nameof(ITranslateComponent.Z)) return;
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

            private void onSomethingChanged()
            {
                _onLayerChanged(_layer);
            }
        }

        public AGSDisplayList(IGameState gameState, IInput input, 
                              IMatrixUpdater matrixUpdater, IAGSRoomTransitions roomTransitions)
        {
            _matrixUpdater = matrixUpdater;
            _roomTransitions = roomTransitions;
            _gameState = gameState;
            _input = input;
            _cache = new ConcurrentDictionary<IViewport, ViewportDisplayList>();
            _viewportSubscribers = new ConcurrentDictionary<IViewport, ViewportSubscriber>();
            _entitySubscribers = new ConcurrentDictionary<string, EntitySubscriber>();
            _comparer = new RenderOrderSelector();

            gameState.UI.OnListChanged.Subscribe(onUiChanged);
            subscribeObjects(gameState.UI);
            subscribeRoom();
            onSomethingChanged();
        }

        public List<IObject> GetDisplayList(IViewport viewport)
        {
            _viewportSubscribers.GetOrAdd(viewport, v => new ViewportSubscriber(v, onEverythingChanged));
            if (_gameState.Room != _lastRoom)
            {
                unsubscribeLastRoom();
                subscribeRoom();
                onEverythingChanged();
            }
            _cache.TryGetValue(viewport, out var displayList);
            return displayList?.DisplayList ?? _emptyList;
		}

        public IObject GetCursor() => _cursor;

        public void Update()
        {
            _cursor = _input.Cursor;
            _alreadyPrepared.Clear();
            bool isDirty = _isDirty || _roomTransitions.State == RoomTransitionState.PreparingNewRoomRendering;
            _isDirty = false;

            _matrixUpdater.ClearCache();
            foreach (var viewport in _viewportSubscribers.Keys)
            {
                ViewportDisplayList list;
                if (isDirty)
                {
                    list = _cache.GetOrAdd(viewport, initDisplayList);
                    updateDisplayList(viewport, list);
                    _cache[viewport] = list;
                }
                else
                {
                    list = _cache.GetOrAdd(viewport, initAndUpdateDisplayList);
                }
                foreach (var item in list.DisplayList)
                {
                    if (_alreadyPrepared.Add(item.ID))
                    {
                        _matrixUpdater.RefreshMatrix(item);
                    }
                }
            }
        }

        private ViewportDisplayList initDisplayList(IViewport viewport)
        {
            return new ViewportDisplayList();
        }

        private ViewportDisplayList initAndUpdateDisplayList(IViewport viewport)
        {
            var displayList = initDisplayList(viewport);
            updateDisplayList(viewport, displayList);
            return displayList;
        }

        private void updateDisplayList(IViewport viewport, ViewportDisplayList displayList)
        {
            var settings = viewport.DisplayListSettings;
            var room = viewport.RoomProvider.Room;

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

            displayList.Sort();
        }

        private void addDebugDrawArea(ViewportDisplayList displayList, IArea area, IViewport viewport)
		{
			if (area.Mask.DebugDraw == null) return;
			addToDisplayList(displayList, area.Mask.DebugDraw, viewport);
		}

        private void addToDisplayList(ViewportDisplayList displayList, IObject obj, IViewport viewport)
		{
            if (!viewport.IsObjectVisible(obj))
			{
                return;
			}

            var layer = displayList.DisplayListPerLayer.GetOrAdd(obj?.RenderLayer?.Z ?? 0, _ => new LayerDisplayList(_comparer));
            if (!layer.IsDirty) return;
            layer.Items.Add(obj);
		}

        private void onUiChanged(AGSHashSetChangedEventArgs<IObject> args)
        {
            if (args.ChangeType == ListChangeType.Add) subscribeObjects(args.Items);
            else unsubscribeObjects(args.Items);
            onEverythingChanged();
        }

        private void onRoomObjectsChanged(AGSHashSetChangedEventArgs<IObject> args)
        {
            if (args.ChangeType == ListChangeType.Add) subscribeObjects(args.Items);
            else unsubscribeObjects(args.Items);
            onEverythingChanged();
        }

        private void subscribeObjects(IEnumerable<IObject> items)
        {
            foreach (var item in items) subscribeEntity(item);
        }

        private void unsubscribeObjects(IEnumerable<IObject> items)
        {
            foreach (var item in items) unsubscribeEntity(item);
        }

        private void onAreaListChanged(AGSListChangedEventArgs<IArea> args)
        {
            if (args.ChangeType == ListChangeType.Add) subscribeAreas(args.Items.Select(a => a.Item));
            else unsubscribeAreas(args.Items.Select(a => a.Item));

            onEverythingChanged();
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

        private void onEverythingChanged()
        {
            foreach (var list in _cache.Values)
            {
                foreach (var layerList in list.DisplayListPerLayer.Values)
                {
                    layerList.IsDirty = true;
                }
            }
            onSomethingChanged();
        }

        private void onLayerChanged(int z)
        {
            foreach (var list in _cache.Values)
            {
                list.DisplayListPerLayer.TryGetValue(z, out var layerList);
                if (layerList != null) layerList.IsDirty = true;
            }
            onSomethingChanged();
        }

        private void onRoomPropertyChanged(object sender, PropertyChangedEventArgs args)
        {
            if (args.PropertyName != nameof(IRoom.Background) && args.PropertyName != nameof(IRoom.RoomLimitsProvider)
                && args.PropertyName != nameof(IRoom.ShowPlayer)) return;
            onEverythingChanged();
            if (args.PropertyName == nameof(IRoom.Background))
            {
                unsubscribeEntity(_lastRoomBackground);
                _lastRoomBackground = _lastRoom.Background;
                subscribeEntity(_lastRoom.Background);
            }
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
            subscribeEntity(room.Background);
            subscribeObjects(room.Objects);
            subscribeAreas(room.Areas);
        }

        private void unsubscribeLastRoom()
        {
            var lastRoom = _lastRoom;
            if (lastRoom == null) return;

            lastRoom.Objects.OnListChanged.Unsubscribe(onRoomObjectsChanged);
            lastRoom.PropertyChanged -= onRoomPropertyChanged;
            unsubscribeEntity(lastRoom.Background);
            unsubscribeObjects(lastRoom.Objects);
            unsubscribeAreas(lastRoom.Areas);
        }

        private static API.IComponentBinding bind<TComponent>(IEntity entity, PropertyChangedEventHandler ev) where TComponent : API.IComponent
        {
            return entity.Bind<TComponent>(c => c.PropertyChanged += ev, c => c.PropertyChanged -= ev);
        }

        private void subscribeEntity(IEntity entity)
        {
            if (entity == null) return;
            EntitySubscriber subscriber = new EntitySubscriber(entity, onLayerChanged);
            _entitySubscribers[entity.ID] = subscriber;
        }

        private void unsubscribeEntity(IEntity entity)
        {
            if (entity == null) return;
            _entitySubscribers.TryRemove(entity.ID, out var subscriber);
            subscriber?.Unsubscribe();
        }

        private void subscribeArea(IArea area)
        {
            if (area == null) return;
            subscribeEntity(area.Mask?.DebugDraw);
            subscribeEntity(area);
        }

        private void unsubscribeArea(IArea area)
        {
            if (area == null) return;
            unsubscribeEntity(area.Mask?.DebugDraw);
            unsubscribeEntity(area);
        }
    }
}