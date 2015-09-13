using System;
using API;
using Engine;
using System.Threading.Tasks;
using System.Drawing;

namespace DemoGame
{
	public class EmptyStreet
	{
		private IRoom _room;
		private const string _baseFolder = "../../Assets/Rooms/EmptyStreet/";

		public EmptyStreet(IPlayer player, IViewport viewport, IGameEvents gameEvents)
		{
			AGSEdges edges = new AGSEdges (new AGSEdge { Value = 10f },
				                 new AGSEdge { Value = 310f }, new AGSEdge { Value = 190f },
				                 new AGSEdge { Value = 10f });
			edges.Left.OnEdgeCrossed.Subscribe(onLeftEdgeCrossed);
			edges.Right.OnEdgeCrossed.Subscribe(onRightEdgeCrossed);
			_room = new AGSRoom ("Empty Street", player, viewport, edges, gameEvents);
		}
			
		public IRoom Load(IGraphicsFactory factory)
		{
			AGSObject bg = new AGSObject (new AGSSprite ());
			bg.Image = factory.LoadImage(_baseFolder + "bg.png");
			_room.Background = bg;

			AGSMaskLoader maskLoader = new AGSMaskLoader (factory);
			_room.WalkableAreas.Add(new AGSArea { Mask = maskLoader.Load(_baseFolder + "walkable1.png") });
			_room.WalkableAreas.Add(new AGSArea { Mask = maskLoader.Load(_baseFolder + "walkable2.png") });

			return _room;
		}

		private void onLeftEdgeCrossed(object sender, EventArgs args)
		{
		}

		private void onRightEdgeCrossed(object sender, EventArgs args)
		{
		}
	}
}

