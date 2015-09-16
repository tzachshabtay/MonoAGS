using System;
using AGS.API;
using AGS.Engine;
using System.Threading.Tasks;
using System.Drawing;

namespace DemoGame
{
	public class EmptyStreet
	{
		private readonly IRoom _room;
		private readonly IPlayer _player;
		private const string _baseFolder = "../../Assets/Rooms/EmptyStreet/";

		public EmptyStreet(IPlayer player, IViewport viewport, IGameEvents gameEvents)
		{
			_player = player;
			AGSEdges edges = new AGSEdges (new AGSEdge { Value = 20f },
				                 new AGSEdge { Value = 310f }, new AGSEdge { Value = 190f },
				                 new AGSEdge { Value = 10f });
			edges.Left.OnEdgeCrossed.Subscribe(onLeftEdgeCrossed);
			edges.Right.OnEdgeCrossed.Subscribe(onRightEdgeCrossed);
			_room = new AGSRoom ("Empty Street", player, viewport, edges, gameEvents);
		}
			
		public IRoom Load(IGameFactory factory)
		{
			IObject bg = factory.GetObject();
			bg.Image = factory.Graphics.LoadImage(_baseFolder + "bg.png");
			_room.Background = bg;

			AGSMaskLoader maskLoader = new AGSMaskLoader (factory);
			_room.WalkableAreas.Add(new AGSArea { Mask = maskLoader.Load(_baseFolder + "walkable1.png") });
			_room.WalkableAreas.Add(new AGSArea { Mask = maskLoader.Load(_baseFolder + "walkable2.png") });

			return _room;
		}

		private void onLeftEdgeCrossed(object sender, EventArgs args)
		{
			_player.Character.ChangeRoom(Rooms.TrashcanStreet, 310);
		}

		private void onRightEdgeCrossed(object sender, EventArgs args)
		{
			_player.Character.ChangeRoom(Rooms.BrokenCurbStreet, 30);
		}
	}
}

