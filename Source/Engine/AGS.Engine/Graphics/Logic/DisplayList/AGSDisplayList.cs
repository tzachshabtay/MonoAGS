using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using AGS.API;

namespace AGS.Engine
{
    public partial class AGSDisplayList : IDisplayList
    {
        private readonly IGameState _gameState;
        private readonly IComparer<IObject> _comparer;
        private readonly List<IObject> _emptyList = new List<IObject>(1);
        private readonly IMatrixUpdater _matrixUpdater;

        private readonly ConcurrentDictionary<IViewport, ViewportDisplayList> _cache;
        private readonly ConcurrentDictionary<IViewport, ViewportSubscriber> _viewportSubscribers;
        private readonly ConcurrentDictionary<string, EntitySubscriber> _entitySubscribers;

        private IRoom _lastRoom;
        private IObject _lastRoomBackground;
        private bool _isDirty;
        private IObject _cursor;

        public AGSDisplayList(IGameState gameState, IMatrixUpdater matrixUpdater)
        {
            _matrixUpdater = matrixUpdater;
            _gameState = gameState;
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

        public void Update(bool forceUpdate)
        {
            _cursor = _gameState.Cursor;
            if (forceUpdate) onEverythingChanged();
            bool isDirty = _isDirty;
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
                    _matrixUpdater.RefreshMatrix(item);
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
                if (room.Background != null) addToDisplayList(displayList, room.Background, viewport);

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

        private void onRoomPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs args)
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

        private static IComponentBinding bind<TComponent>(IEntity entity, System.ComponentModel.PropertyChangedEventHandler ev) where TComponent : IComponent
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
