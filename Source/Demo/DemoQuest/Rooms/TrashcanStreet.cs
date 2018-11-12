using System.Threading.Tasks;
using AGS.API;
using AGS.Engine;

namespace DemoGame
{
	public class TrashcanStreet
	{
		private IRoom _room;
        private ICharacter _player;
		private IGame _game;

		private const string _baseFolder = "Rooms/TrashcanStreet/";

		public async Task<IRoom> LoadAsync(IGame game)
		{
			_game = game;
			_player = _game.State.Player;
            _game.Events.OnSavedGameLoad.Subscribe(onSavedGameLoaded);
			IGameFactory factory = game.Factory;
			_room = factory.Room.GetRoom("Trashcan Street", 20f, 310f, 190f, 10f);
			IObject bg = factory.Object.GetObject("Trashcan Street BG");
			bg.Image = await factory.Graphics.LoadImageAsync(_baseFolder + "bg.png");
			_room.Background = bg;

            await factory.Room.GetAreaAsync(_baseFolder + "walkable.png", _room, isWalkable: true);
            factory.Room.CreateScaleArea(_room.Areas[0], 0.50f, 0.90f);

			await factory.Object.GetHotspotAsync(_baseFolder + "HoleHotspot.png", "Hole", _room);
			await factory.Object.GetHotspotAsync(_baseFolder + "sidewalkHotspot.png", "Sidewalk", _room);
			await factory.Object.GetHotspotAsync(_baseFolder + "SignHotspot.png", "Sign", _room);
			await factory.Object.GetHotspotAsync(_baseFolder + "trashCansHotspot.png", "Trashcans", _room);
            var roadHotspot = await factory.Object.GetHotspotAsync(_baseFolder + "roadHotspot.png", "Road", _room);
            roadHotspot.Z = 100f;

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

        private async void onLeftEdgeCrossed()
		{
            await _player.ChangeRoomAsync(Rooms.DarsStreet.Result, 490);
		}

		private async void onRightEdgeCrossed()
		{
			await _player.ChangeRoomAsync(Rooms.EmptyStreet.Result, 30);
		}

		private void onBeforeFadeIn()
		{
			_player.PlaceOnWalkableArea();
		}
	}
}
