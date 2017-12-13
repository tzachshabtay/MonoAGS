using System.Linq;
using AGS.API;
using System.Collections.Generic;

namespace AGS.Engine
{
    [PropertyFolder]
	public class AGSRoom : IRoom
	{
        private ICharacter _player => _state.Player;
        private IObject _background;
		private readonly IAGSEdges _edges;
		private readonly IGameState _state;
		private readonly IGameEvents _gameEvents;
        private readonly IInput _input;

		public AGSRoom (string id, IAGSEdges edges, IGameEvents gameEvents,
                        IRoomEvents roomEvents, IGameState state, ICustomProperties properties,
                        IRoomLimitsProvider roomLimitsProvider, IInput input)
		{
			_state = state;
            _input = input;
            RoomLimitsProvider = roomLimitsProvider;
			_gameEvents = gameEvents;
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

		public string ID { get; }

		public ICustomProperties Properties { get; }

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
            get => _background;
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

		public IConcurrentHashSet<IObject> Objects { get; }

		public IList<IArea> Areas { get; }

        public IEdges Edges => _edges;

        [Property(Browsable = false)]
		public IRoomEvents Events { get; }

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

        public override string ToString() => ID ?? base.ToString();

        #endregion

        private void onRepeatedlyExecute()
		{
            if (_player == null || _player.Room != this) return;
			_edges.OnRepeatedlyExecute(_player);
		}
    }
}

