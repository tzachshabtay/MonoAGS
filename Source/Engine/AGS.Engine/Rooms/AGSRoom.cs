using System;
using System.Linq;
using AGS.API;
using System.Collections.Generic;

namespace AGS.Engine
{
	public class AGSRoom : IRoom
	{
        private ICharacter _player { get { return _state.Player; } }
		private IObject _background;
		private readonly IAGSEdges _edges;
		private readonly RenderOrderSelector _sorter;
		private readonly IGameState _state;
		private readonly IGameEvents _gameEvents;
        private List<IObject> _visibleObjectsWithUi = new List<IObject>(), _visibleObjectsWithoutUi = new List<IObject>();

		public AGSRoom (string id, IViewport viewport, IAGSEdges edges, IGameEvents gameEvents,
                        IRoomEvents roomEvents, IGameState state, ICustomProperties properties,
                        IRoomLimitsProvider roomLimitsProvider)
		{
			_sorter = new RenderOrderSelector { Backwards = true };
			_state = state;
            RoomLimitsProvider = roomLimitsProvider;
			_gameEvents = gameEvents;
			Viewport = viewport;
			Events = roomEvents;
			ID = id;
			Objects = new AGSConcurrentHashSet<IObject> ();
			Areas = new List<IArea> ();
			ShowPlayer = true;
			_edges = edges;
			Properties = properties;
			gameEvents.OnRepeatedlyExecute.Subscribe(onRepeatedlyExecute);
		}

		#region IRoom implementation

		public string ID { get; private set; }

		public ICustomProperties Properties { get; private set; }

		public bool ShowPlayer { get; set; }

		public IAudioClip MusicOnLoad { get; set; }

        public IRoomLimitsProvider RoomLimitsProvider { get; set; }

        public RectangleF Limits
        {
            get
            {
                var provider = RoomLimitsProvider;
                if (provider == null) return RoomCustomLimits.MaxLimits;
                return RoomLimitsProvider.ProvideRoomLimits(this);
            }
        }

		public IObject Background 
		{ 
			get { return _background; } 
			set 
			{ 
				_background = value; 
				if (_background != null && _background.RenderLayer == AGSLayers.Foreground)
				{
					_background.RenderLayer = AGSLayers.Background;
				}
				if (_background != null)
				{
					_background.Anchor = new PointF ();
				}
			} 
		}

		public IConcurrentHashSet<IObject> Objects { get; private set; }

		public IList<IArea> Areas { get; private set; }

		public IViewport Viewport { get; private set; }

		public IEdges Edges { get { return _edges; } }

		public IRoomEvents Events { get; private set; }

        public IEnumerable<IArea> GetMatchingAreas(PointF point, string entityId)
        {
            return Areas.Where(area =>
            {
                if (!area.Enabled || !area.IsInArea(point)) return false;
                if (entityId == null) return true;
                IAreaRestriction areaRestriction = area.GetComponent<IAreaRestriction>();
                return (areaRestriction == null || !areaRestriction.IsRestricted(entityId));
            });
        }

		public IEnumerable<IObject> GetVisibleObjectsFrontToBack(bool includeUi = true)
		{
            return includeUi ? _visibleObjectsWithUi : _visibleObjectsWithoutUi;
		}

		public IObject GetObjectAt(float x, float y, bool onlyEnabled = true, bool includeUi = true)
		{
            foreach (IObject obj in GetVisibleObjectsFrontToBack())
            {
                if (onlyEnabled && !obj.Enabled)
                    continue;

                if (!obj.CollidesWith(x, y)) continue;

                if (!hasFocus(obj)) continue;

                return obj;
            }
            return null;
		}

		public TObject Find<TObject>(string id) where TObject : class, IObject
		{
			return Objects.FirstOrDefault(o => o.ID == id) as TObject;
		}

		public void Dispose()
		{
			_gameEvents.OnRepeatedlyExecute.Unsubscribe(onRepeatedlyExecute);
			foreach (var obj in Objects)
			{
				obj.Dispose();
			}
		}

		public override string ToString()
		{
			return ID ?? base.ToString();
		}

        #endregion

        private bool hasFocus(IObject obj)
        {
            var focusedWindow = _state.FocusedUI.FocusedWindow;
            if (focusedWindow == null) return true;
            while (obj != null)
            {
                if (obj == focusedWindow) return true;
                obj = obj.TreeNode.Parent;
            }
            return false;
        }

		private void onRepeatedlyExecute(object sender, EventArgs args)
		{
            if (_player == null || _player.Room != this) return;
            cacheVisibleObjects();
			_edges.OnRepeatedlyExecute(_player);
		}

        private void cacheVisibleObjects()
        { 
            List<IObject> visibleObjects = Objects.Where(o => o.Visible && (ShowPlayer || o != _player)).ToList();
            List<IObject> visibleWithUi = new List<IObject>(visibleObjects);
            visibleWithUi.AddRange(_state.UI.Where(o => o.Visible));
            visibleObjects.Sort(_sorter);
            visibleWithUi.Sort(_sorter);
            _visibleObjectsWithUi = visibleWithUi;
            _visibleObjectsWithoutUi = visibleObjects;
        }
    }
}

