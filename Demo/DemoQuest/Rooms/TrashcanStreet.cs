using System;
using System.Threading.Tasks;
using AGS.API;
using AGS.Engine;

namespace DemoGame
{
	public class TrashcanStreet
	{
		private IRoom _room;
		private IPlayer _player;
		private IGame _game;

		private const string _baseFolder = "../../Assets/Rooms/TrashcanStreet/";

		public async Task<IRoom> LoadAsync(IGame game)
		{
			_game = game;
			_player = _game.State.Player;
			_game.Events.OnSavedGameLoad.Subscribe((sender, e) => onSavedGameLoaded());
			IGameFactory factory = game.Factory;
			_room = factory.Room.GetRoom("Trashcan Street", 20f, 310f, 190f, 10f);
			IObject bg = factory.Object.GetObject("Trashcan Street BG");
			bg.Image = await factory.Graphics.LoadImageAsync(_baseFolder + "bg.png");
			_room.Background = bg;

			AGSMaskLoader maskLoader = new AGSMaskLoader (factory, new ResourceLoader());
			_room.WalkableAreas.Add(new AGSArea { Mask = await maskLoader.LoadAsync(_baseFolder + "walkable.png") });
			_room.ScalingAreas.Add(AGSScalingArea.Create(_room.WalkableAreas[0], 0.50f, 0.90f));

			_room.Objects.Add(await factory.Object.GetHotspotAsync(_baseFolder + "HoleHotspot.png", "Hole"));
			_room.Objects.Add(await factory.Object.GetHotspotAsync(_baseFolder + "roadHotspot.png", "Road"));
			_room.Objects.Add(await factory.Object.GetHotspotAsync(_baseFolder + "sidewalkHotspot.png", "Sidewalk"));
			_room.Objects.Add(await factory.Object.GetHotspotAsync(_baseFolder + "SignHotspot.png", "Sign"));
			_room.Objects.Add(await factory.Object.GetHotspotAsync(_baseFolder + "trashCansHotspot.png", "Trashcans"));

			subscribeEvents();
			return _room;
		}

		private void onSavedGameLoaded()
		{
			_player = _game.State.Player;
			_room = Rooms.Find(_game.State, _room);
			subscribeEvents();
		}

		private void subscribeEvents()
		{
			_room.Edges.Left.OnEdgeCrossed.Subscribe(onLeftEdgeCrossed);
			_room.Edges.Right.OnEdgeCrossed.Subscribe(onRightEdgeCrossed);
			_room.Events.OnBeforeFadeIn.Subscribe(onBeforeFadeIn);
		}

		private void onLeftEdgeCrossed(object sender, AGSEventArgs args)
		{
			_player.Character.ChangeRoom(Rooms.DarsStreet.Result, 490);
		}

		private void onRightEdgeCrossed(object sender, AGSEventArgs args)
		{
			_player.Character.ChangeRoom(Rooms.EmptyStreet.Result, 30);
		}

		private void onBeforeFadeIn(object sender, AGSEventArgs args)
		{
			_player.Character.PlaceOnWalkableArea();
		}
	}
}

