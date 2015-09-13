using System;
using System.Linq;
using API;
using System.Collections.Generic;
using System.Drawing;
using System.Collections.Concurrent;

namespace Engine
{
	public class AGSRoom : IRoom
	{
		private IPlayer _player;
		private IObject _background;
		private IAGSEdges _edges;

		public AGSRoom (string id, IPlayer player, IViewport viewport, IAGSEdges edges, IGameEvents gameEvents)
		{
			this._player = player;
			Viewport = viewport;
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

		public IEnumerable<IObject> GetVisibleObjectsFrontToBack()
		{
			return Objects.Where (o => o.Visible && (ShowPlayer || o != _player.Character)).
				OrderByDescending(o => o.RenderLayer == null ? 0 : o.RenderLayer.Z).
				ThenByDescending(o => o.Z);
		}

		public IObject GetHotspotAt(float x, float y)
		{
			return getObjectAt (x, y, true);
		}

		public IObject GetObjectAt(float x, float y)
		{
			return getObjectAt (x, y, false);
		}

		#endregion

		private IObject getObjectAt(float x, float y, bool onlyHotspots)
		{
			foreach (IObject obj in GetVisibleObjectsFrontToBack()) 
			{
				if (onlyHotspots && !obj.Enabled)
					continue;

				ISquare boundingBox = obj.BoundingBox;
				if (boundingBox == null)
					continue;
				if (boundingBox.Contains (new AGSPoint (x, y)))
					return obj;
				//Rectangle rect = new Rectangle ((int)obj.X, (int)obj.Y, (int)obj.Width, (int)obj.Height);

				//if (rect.Contains ((int)x, (int)y))
				//	return obj;
			}
			return null;
		}

		private void onRepeatedlyExecute(object sender, EventArgs args)
		{
			if (_player.Character.Room != this) return;
			_edges.OnRepeatedlyExecute(_player.Character);
		}

	}
}

