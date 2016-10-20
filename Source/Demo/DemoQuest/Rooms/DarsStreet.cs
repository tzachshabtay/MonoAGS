using System;
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

		private const string _baseFolder = "../../Assets/Rooms/DarsStreet/";

		public async Task<IRoom> LoadAsync(IGame game)
		{
			_game = game;
			_game.Events.OnSavedGameLoad.Subscribe((sender, e) => onSavedGameLoaded());
			_player = _game.State.Player;
			IGameFactory factory = game.Factory;
			_room = factory.Room.GetRoom("Dars Street", 20f, 490f, 190f, 10f);
			IObject bg = factory.Object.GetObject("Dars Street BG");
			IAnimation bgAnimation = await factory.Graphics.LoadAnimationFromFolderAsync(_baseFolder + "bg");
			bgAnimation.Frames[0].MinDelay = 1;
			bgAnimation.Frames[0].MaxDelay = 120;
			bg.StartAnimation(bgAnimation);
			_room.Background = bg;

			AGSMaskLoader maskLoader = new AGSMaskLoader (factory, new ResourceLoader());
            _room.Areas.Add(AGSWalkableArea.Create("DarsStreetWalkable1", await maskLoader.LoadAsync(_baseFolder + "walkable1.png")));
            _room.Areas.Add(AGSWalkableArea.Create("DarsStreetWalkable2", await maskLoader.LoadAsync(_baseFolder + "walkable2.png")));
			AGSScalingArea.Create(_room.Areas[0], 0.35f, 0.75f);
            AGSZoomArea.Create(_room.Areas[0], 1f, 1.2f);
			AGSScalingArea.Create(_room.Areas[1], 0.10f, 0.35f);
			AGSZoomArea.Create(_room.Areas[1], 1.2f, 1.8f);

            _room.Areas.Add(AGSWalkBehindArea.Create("DarsStreetWalkBehind1", await maskLoader.LoadAsync(_baseFolder + "walkbehind1.png")));
			_room.Areas.Add(AGSWalkBehindArea.Create("DarsStreetWalkBehind2", await maskLoader.LoadAsync (_baseFolder + "walkbehind2.png")));
			_room.Areas.Add(AGSWalkBehindArea.Create("DarsStreetWalkBehind3", await maskLoader.LoadAsync (_baseFolder + "walkbehind3.png")));
		
			IObject buildingHotspot = await factory.Object.GetHotspotAsync(_baseFolder + "buildingHotspot.png", "Building");
			IObject doorHotspot = await factory.Object.GetHotspotAsync (_baseFolder + "doorHotspot.png", "Door");
			IObject windowHotspot = await factory.Object.GetHotspotAsync (_baseFolder + "windowHotspot.png", "Window");
			doorHotspot.Z = buildingHotspot.Z - 1;
			windowHotspot.Z = buildingHotspot.Z - 1;
            windowHotspot.Interactions.OnInteract(AGSInteractions.LOOK).SubscribeToAsync(lookOnWindow);

			_room.Objects.Add(await factory.Object.GetHotspotAsync(_baseFolder + "aztecBuildingHotspot.png", "Aztec Building"));
			_room.Objects.Add(buildingHotspot);
			_room.Objects.Add(await factory.Object.GetHotspotAsync(_baseFolder + "carHotspot.png", "Car"));
			_room.Objects.Add(doorHotspot);
			_room.Objects.Add(await factory.Object.GetHotspotAsync(_baseFolder + "fencesHotspot.png", "Fences"));
			_room.Objects.Add(await factory.Object.GetHotspotAsync(_baseFolder + "neonSignHotspot.png", "Neon Sign"));
			_room.Objects.Add(await factory.Object.GetHotspotAsync(_baseFolder + "roadHotspot.png", "Road"));
			_room.Objects.Add(await factory.Object.GetHotspotAsync(_baseFolder + "sidewalkHotspot.png", "Sidewalk"));
			_room.Objects.Add(await factory.Object.GetHotspotAsync(_baseFolder + "skylineHotspot.png", "Skyline"));
			_room.Objects.Add(await factory.Object.GetHotspotAsync(_baseFolder + "trashcansHotspot.png", "Trashcans"));
			_room.Objects.Add(windowHotspot);
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

		private void onRightEdgeCrossed(object sender, AGSEventArgs args)
		{
			_player.ChangeRoom(Rooms.TrashcanStreet.Result, 30);
		}

		private void onBeforeFadeIn(object sender, AGSEventArgs args)
		{
			_player.PlaceOnWalkableArea();
		}

		private async Task addLampPosts(IGameFactory factory)
		{
			PointF parallaxSpeed = new PointF (1.4f, 1f);
			AGSRenderLayer parallaxLayer = new AGSRenderLayer (-50, parallaxSpeed);
			var image = await factory.Graphics.LoadImageAsync(_baseFolder + "lampPost.png");
			var singleFrame = new AGSSingleFrameAnimation (image, factory.Graphics);
			const int numLampPosts = 3;

			for (int index = 0; index < numLampPosts; index++)
			{
				IObject lampPost = factory.Object.GetObject("Lamp Post " + index);
				lampPost.X = 200f * index + 30f;
				lampPost.Y = -130f;
				lampPost.RenderLayer = parallaxLayer;
				lampPost.StartAnimation(singleFrame);
				_room.Objects.Add(lampPost);
			}
		}

		private async Task lookOnWindow(object sender, AGSEventArgs args)
		{
			_room.Viewport.Camera.Enabled = false;

			float scaleX = _room.Viewport.ScaleX;
			float scaleY = _room.Viewport.ScaleY;
			float angle = _room.Viewport.Angle;
			float x = _room.Viewport.X;
			float y = _room.Viewport.Y;

			Tween zoomX = _room.Viewport.TweenScaleX(4f, 2f);
			Tween zoomY = _room.Viewport.TweenScaleY(4f, 2f);
			Task rotate = _room.Viewport.TweenAngle(0.1f, 1f, Ease.QuadOut).Task.
				ContinueWith(t => _room.Viewport.TweenAngle(angle, 1f, Ease.QuadIn).Task);
			Tween translateX = _room.Viewport.TweenX(240f, 2f);
			Tween translateY = _room.Viewport.TweenY(100f, 2f);

			await Task.WhenAll(zoomX.Task, zoomY.Task, rotate, translateX.Task, translateY.Task);
			await Task.Delay(100);
			await _player.SayAsync("Hmmm, nobody seems to be home...");
			await Task.Delay(100);

			zoomX = _room.Viewport.TweenScaleX(scaleX, 2f);
			zoomY = _room.Viewport.TweenScaleY(scaleY, 2f);
			rotate = _room.Viewport.TweenAngle(0.1f, 1f, Ease.QuadIn).Task.
				ContinueWith(t => _room.Viewport.TweenAngle(angle, 1f, Ease.QuadOut).Task);
			translateX = _room.Viewport.TweenX(x, 2f);
			translateY = _room.Viewport.TweenY(y, 2f);

			await Task.WhenAll(zoomX.Task, zoomY.Task, rotate, translateX.Task, translateY.Task);
			_room.Viewport.Camera.Enabled = true;
		}
	}
}

