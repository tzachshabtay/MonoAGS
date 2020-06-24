﻿using System.Linq;
using AGS.API;
using System.Collections.Generic;
using System.ComponentModel;

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

#pragma warning disable CS0067
        public event PropertyChangedEventHandler PropertyChanged;
#pragma warning restore CS0067

        public AGSRoom (string id, IAGSEdges edges, IGameEvents gameEvents,
                        IRoomEvents roomEvents, IGameState state, ICustomProperties properties,
                        IRoomLimitsProvider roomLimitsProvider)
		{
			_state = state;
		    RoomLimitsProvider = roomLimitsProvider;
			_gameEvents = gameEvents;
			Events = roomEvents;
			ID = id;
			Objects = new AGSConcurrentHashSet<IObject>();
            Areas = new AGSBindingList<IArea>(5);
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
				if (_background != null)
				{
				    if (_background.RenderLayer == AGSLayers.Foreground)
				    {
				        _background.RenderLayer = AGSLayers.Background;
				    }
					_background.Pivot = new PointF ();
                    _background.Enabled = false;
				}
			} 
		}

        public Color? BackgroundColor { get; set; }

        public IConcurrentHashSet<IObject> Objects { get; }

		public IAGSBindingList<IArea> Areas { get; }

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

