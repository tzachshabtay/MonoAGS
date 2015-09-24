using System;
using AGS.API;
using AGS.Engine;

namespace DemoGame
{
	public class TrashcanStreet
	{
		private IRoom _room;
		private readonly IPlayer _player;

		private const string _baseFolder = "../../Assets/Rooms/TrashcanStreet/";

		public TrashcanStreet(IPlayer player)
		{
			_player = player;
		}

		public IRoom Load(IGameFactory factory)
		{
			_room = factory.GetRoom("Trashcan Street", 20f, 310f, 190f, 10f);
			_room.Edges.Left.OnEdgeCrossed.Subscribe(onLeftEdgeCrossed);
			_room.Edges.Right.OnEdgeCrossed.Subscribe(onRightEdgeCrossed);
			_room.Events.OnBeforeFadeIn.Subscribe(onBeforeFadeIn);
			IObject bg = factory.GetObject();
			bg.Image = factory.Graphics.LoadImage(_baseFolder + "bg.png");
			_room.Background = bg;

			AGSMaskLoader maskLoader = new AGSMaskLoader (factory);
			_room.WalkableAreas.Add(new AGSArea { Mask = maskLoader.Load(_baseFolder + "walkable.png") });
			_room.ScalingAreas.Add(AGSScalingArea.Create(_room.WalkableAreas[0], 0.50f, 0.90f));

			_room.Objects.Add(factory.GetHotspot(_baseFolder + "HoleHotspot.png", "Hole"));
			_room.Objects.Add(factory.GetHotspot(_baseFolder + "roadHotspot.png", "Road"));
			_room.Objects.Add(factory.GetHotspot(_baseFolder + "sidewalkHotspot.png", "Sidewalk"));
			_room.Objects.Add(factory.GetHotspot(_baseFolder + "SignHotspot.png", "Sign"));
			_room.Objects.Add(factory.GetHotspot(_baseFolder + "trashCansHotspot.png", "Trashcans"));

			return _room;
		}

		private void onLeftEdgeCrossed(object sender, AGSEventArgs args)
		{
			_player.Character.ChangeRoom(Rooms.DarsStreet, 490);
		}

		private void onRightEdgeCrossed(object sender, AGSEventArgs args)
		{
			_player.Character.ChangeRoom(Rooms.EmptyStreet, 30);
		}

		private void onBeforeFadeIn(object sender, AGSEventArgs args)
		{
			_player.Character.PlaceOnWalkableArea();
		}
	}
}

