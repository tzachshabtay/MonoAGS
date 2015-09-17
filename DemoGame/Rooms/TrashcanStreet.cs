using System;
using AGS.API;
using AGS.Engine;

namespace DemoGame
{
	public class TrashcanStreet
	{
		private readonly IRoom _room;
		private readonly IPlayer _player;
		private const string _baseFolder = "../../Assets/Rooms/TrashcanStreet/";

		public TrashcanStreet(IPlayer player, IViewport viewport, IGameEvents gameEvents)
		{
			_player = player;
			AGSEdges edges = new AGSEdges (new AGSEdge { Value = 20f },
				new AGSEdge { Value = 310f }, new AGSEdge { Value = 190f },
				new AGSEdge { Value = 10f });
			edges.Left.OnEdgeCrossed.Subscribe(onLeftEdgeCrossed);
			edges.Right.OnEdgeCrossed.Subscribe(onRightEdgeCrossed);
			_room = new AGSRoom ("Trashcan Street", player, viewport, edges, gameEvents);
		}

		public IRoom Load(IGameFactory factory)
		{
			IObject bg = factory.GetObject();
			bg.Image = factory.Graphics.LoadImage(_baseFolder + "bg.png");
			_room.Background = bg;

			AGSMaskLoader maskLoader = new AGSMaskLoader (factory);
			_room.WalkableAreas.Add(new AGSArea { Mask = maskLoader.Load(_baseFolder + "walkable.png") });

			_room.Objects.Add(factory.GetHotspot(_baseFolder + "HoleHotspot.png", "Hole"));
			_room.Objects.Add(factory.GetHotspot(_baseFolder + "roadHotspot.png", "Road"));
			_room.Objects.Add(factory.GetHotspot(_baseFolder + "sidewalkHotspot.png", "Sidewalk"));
			_room.Objects.Add(factory.GetHotspot(_baseFolder + "SignHotspot.png", "Sign"));
			_room.Objects.Add(factory.GetHotspot(_baseFolder + "trashCansHotspot.png", "Trashcans"));

			return _room;
		}

		private void onLeftEdgeCrossed(object sender, EventArgs args)
		{
			_player.Character.ChangeRoom(Rooms.DarsStreet, 490);
		}

		private void onRightEdgeCrossed(object sender, EventArgs args)
		{
			_player.Character.ChangeRoom(Rooms.EmptyStreet, 30);
		}
	}
}

