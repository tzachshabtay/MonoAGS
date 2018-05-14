using AGS.API;
using AGS.Engine;
using System.Threading.Tasks;

namespace DemoGame
{
	public class DarsStreet
	{
		private IRoom _room;
        private ICharacter _player;
		private IGame _game;

		private const string _baseFolder = "Rooms/DarsStreet/";

		public async Task<IRoom> LoadAsync(IGame game)
		{
			_game = game;
            _game.Events.OnSavedGameLoad.Subscribe(onSavedGameLoaded);
			_player = _game.State.Player;
			IGameFactory factory = game.Factory;
			_room = factory.Room.GetRoom("Dars Street", 20f, 490f, 190f, 10f);
			IObject bg = factory.Object.GetObject("Dars Street BG");
			IAnimation bgAnimation = await factory.Graphics.LoadAnimationFromFolderAsync(_baseFolder + "bg");
			bgAnimation.Frames[0].MinDelay = 1;
			bgAnimation.Frames[0].MaxDelay = 120;
			bg.StartAnimation(bgAnimation);
			_room.Background = bg;

            var device = AGSGame.Device;
            await factory.Room.GetAreaAsync(_baseFolder + "walkable1.png", _room, isWalkable: true);
            await factory.Room.GetAreaAsync(_baseFolder + "walkable2.png", _room, isWalkable: true);
            factory.Room.CreateScaleArea(_room.Areas[0], 0.35f, 0.75f);
            factory.Room.CreateZoomArea(_room.Areas[0], 1f, 1.2f);
            factory.Room.CreateScaleArea(_room.Areas[1], 0.10f, 0.35f);
            factory.Room.CreateZoomArea(_room.Areas[1], 1.2f, 1.8f);

            await factory.Room.GetAreaAsync(_baseFolder + "walkbehind1.png", _room, isWalkBehind: true);
			await factory.Room.GetAreaAsync(_baseFolder + "walkbehind2.png", _room, isWalkBehind: true);
			await factory.Room.GetAreaAsync(_baseFolder + "walkbehind3.png", _room, isWalkBehind: true);
		
			IObject buildingHotspot = await factory.Object.GetHotspotAsync(_baseFolder + "buildingHotspot.png", "Building", _room);
			IObject doorHotspot = await factory.Object.GetHotspotAsync (_baseFolder + "doorHotspot.png", "Door", _room);
			IObject windowHotspot = await factory.Object.GetHotspotAsync (_baseFolder + "windowHotspot.png", "Window", _room);
			doorHotspot.Z = buildingHotspot.Z - 1;
			windowHotspot.Z = buildingHotspot.Z - 1;
            windowHotspot.GetComponent<IHotspotComponent>().Interactions.OnInteract(AGSInteractions.LOOK).SubscribeToAsync(lookOnWindow);

            await factory.Object.GetHotspotAsync(_baseFolder + "aztecBuildingHotspot.png", "Aztec Building",_room);
            await factory.Object.GetHotspotAsync(_baseFolder + "carHotspot.png", "Car", _room);
            await factory.Object.GetHotspotAsync(_baseFolder + "fencesHotspot.png", "Fences", _room);
            await factory.Object.GetHotspotAsync(_baseFolder + "neonSignHotspot.png", "Neon Sign", _room);
            await factory.Object.GetHotspotAsync(_baseFolder + "roadHotspot.png", "Road", _room);
            await factory.Object.GetHotspotAsync(_baseFolder + "sidewalkHotspot.png", "Sidewalk", _room);
            await factory.Object.GetHotspotAsync(_baseFolder + "skylineHotspot.png", "Skyline", _room);
            await factory.Object.GetHotspotAsync(_baseFolder + "trashcansHotspot.png", "Trashcans", _room);
			await addLampPosts(factory);

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
            _room.Edges.Right.OnEdgeCrossed.Subscribe(onRightEdgeCrossed);
			_room.Events.OnBeforeFadeIn.Subscribe(onBeforeFadeIn);
		}

        private async void onRightEdgeCrossed()
		{
			await _player.ChangeRoomAsync(Rooms.TrashcanStreet.Result, 30);
		}

		private void onBeforeFadeIn()
		{
			_player.PlaceOnWalkableArea();
		}

		private async Task addLampPosts(IGameFactory factory)
		{
			PointF parallaxSpeed = new PointF (1.4f, 1f);
			AGSRenderLayer parallaxLayer = new AGSRenderLayer (-50, parallaxSpeed);
			var image = await factory.Graphics.LoadImageAsync(_baseFolder + "lampPost.png");
			const int numLampPosts = 3;

			for (int index = 0; index < numLampPosts; index++)
			{
                IObject lampPost = factory.Object.GetAdventureObject("Lamp Post " + index, _room);
                lampPost.DisplayName = "Lamp Post";
                lampPost.IsPixelPerfect = true;
				lampPost.X = 200f * index + 30f;
				lampPost.Y = -130f;
				lampPost.RenderLayer = parallaxLayer;
                lampPost.Image = image;
			}
		}

		private async Task lookOnWindow(ObjectEventArgs args)
		{
            var viewport = _game.State.Viewport;
			viewport.Camera.Enabled = false;

			float scaleX = viewport.ScaleX;
			float scaleY = viewport.ScaleY;
			float angle = viewport.Angle;
			float x = viewport.X;
			float y = viewport.Y;

			Tween zoomX = viewport.TweenScaleX(4f, 2f);
			Tween zoomY = viewport.TweenScaleY(4f, 2f);
			Task rotate = viewport.TweenAngle(6f, 1f, Ease.QuadOut).Task.
				ContinueWith(t => viewport.TweenAngle(angle, 1f, Ease.QuadIn).Task);
			Tween translateX = viewport.TweenX(240f, 2f);
			Tween translateY = viewport.TweenY(100f, 2f);

			await Task.WhenAll(zoomX.Task, zoomY.Task, rotate, translateX.Task, translateY.Task);
			await Task.Delay(100);
			await _player.SayAsync("Hmmm, nobody seems to be home...");
			await Task.Delay(100);

			zoomX = viewport.TweenScaleX(scaleX, 2f);
			zoomY = viewport.TweenScaleY(scaleY, 2f);
			rotate = viewport.TweenAngle(6f, 1f, Ease.QuadIn).Task.
				ContinueWith(t => viewport.TweenAngle(angle, 1f, Ease.QuadOut).Task);
			translateX = viewport.TweenX(x, 2f);
			translateY = viewport.TweenY(y, 2f);

			await Task.WhenAll(zoomX.Task, zoomY.Task, rotate, translateX.Task, translateY.Task);
			viewport.Camera.Enabled = true;
		}
	}
}