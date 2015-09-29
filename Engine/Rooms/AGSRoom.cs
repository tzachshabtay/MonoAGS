using System;
using System.Linq;
using AGS.API;
using System.Collections.Generic;
using System.Drawing;
using System.Collections.Concurrent;

namespace AGS.Engine
{
	public class AGSRoom : IRoom
	{
		private IPlayer _player;
		private IObject _background;
		private IAGSEdges _edges;
		private RenderOrderSelector _sorter;
		private IGameState _state;

		public AGSRoom (string id, IPlayer player, IViewport viewport, IAGSEdges edges, IGameEvents gameEvents,
			IRoomEvents roomEvents, IGameState state)
		{
			this._sorter = new RenderOrderSelector { Backwards = true };
			this._player = player;
			this._state = state;
			Viewport = viewport;
			Events = roomEvents;
			ID = id;
			Objects = new List<IObject> ();
			WalkableAreas = new List<IArea> ();
			WalkBehindAreas = new List<IWalkBehindArea> ();
			ScalingAreas = new List<IScalingArea> ();
			ShowPlayer = true;
			_edges = edges;
			gameEvents.OnRepeatedlyExecute.Subscribe(onRepeatedlyExecute);
		}

		#region IRoom implementation

		public string ID { get; private set; }

		public bool ShowPlayer { get; set; }

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
					_background.Anchor = new AGSPoint ();
				}
			} 
		}

		public IList<IObject> Objects { get; private set; }

		public IList<IArea> WalkableAreas { get; private set; }

		public IList<IWalkBehindArea> WalkBehindAreas { get; private set; }

		public IList<IScalingArea> ScalingAreas { get; private set; }

		public IViewport Viewport { get; private set; }

		public IEdges Edges { get { return _edges; } }

		public IRoomEvents Events { get; private set; }

		public IEnumerable<IObject> GetVisibleObjectsFrontToBack(bool includeUi = true)
		{
			List<IObject> visibleObjects = Objects.Where(o => o.Visible && (ShowPlayer || o != _player.Character)).ToList();
			if (includeUi) visibleObjects.AddRange(_state.UI);
			visibleObjects.Sort(_sorter);
			return visibleObjects;
		}

		public IObject GetObjectAt(float x, float y, bool onlyEnabled = true, bool includeUi = true)
		{
			foreach (IObject obj in GetVisibleObjectsFrontToBack()) 
			{
				if (onlyEnabled && !obj.Enabled)
					continue;

				if (obj.CollidesWith(x, y)) return obj;
			}
			return null;
		}

		#endregion

		private void onRepeatedlyExecute(object sender, EventArgs args)
		{
			if (_player.Character.Room != this) return;
			_edges.OnRepeatedlyExecute(_player.Character);
		}

	}
}

