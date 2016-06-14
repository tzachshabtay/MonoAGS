using System;
using AGS.API;
using AGS.Engine;
using System.Threading.Tasks;

namespace DemoGame
{
	public class DarsStreet
	{
		private IRoom _room;
		private IPlayer _player;
		private IGame _game;

		private const string _baseFolder = "../../Assets/Rooms/DarsStreet/";

		public IRoom Load(IGame game)
		{
			_game = game;
			_game.Events.OnSavedGameLoad.Subscribe((sender, e) => onSavedGameLoaded());
			_player = _game.State.Player;
			IGameFactory factory = game.Factory;
			_room = factory.Room.GetRoom("Dars Street", 20f, 490f, 190f, 10f);
			IObject bg = factory.Object.GetObject("Dars Street BG");
			IAnimation bgAnimation = factory.Graphics.LoadAnimationFromFolder(_baseFolder + "bg");
			bgAnimation.Frames[0].MinDelay = 1;
			bgAnimation.Frames[0].MaxDelay = 120;
			bg.StartAnimation(bgAnimation);
			_room.Background = bg;

			AGSMaskLoader maskLoader = new AGSMaskLoader (factory, new ResourceLoader());
			_room.WalkableAreas.Add(new AGSArea { Mask = maskLoader.Load(_baseFolder + "walkable1.png") });
			_room.WalkableAreas.Add(new AGSArea { Mask = maskLoader.Load(_baseFolder + "walkable2.png") });
			_room.ScalingAreas.Add(AGSScalingArea.Create(_room.WalkableAreas[0], 0.35f, 0.75f));
			_room.ScalingAreas.Add(AGSScalingArea.CreateZoom(_room.WalkableAreas[0], 1.2f, 1f));
			_room.ScalingAreas.Add(AGSScalingArea.Create(_room.WalkableAreas[1], 0.10f, 0.35f));
			_room.ScalingAreas.Add(AGSScalingArea.CreateZoom(_room.WalkableAreas[1], 1.8f, 1.2f));

			_room.WalkBehindAreas.Add(new AGSWalkBehindArea (new AGSArea { Mask = maskLoader.Load(_baseFolder + "walkbehind1.png") }));
			_room.WalkBehindAreas.Add(new AGSWalkBehindArea (new AGSArea { Mask = maskLoader.Load(_baseFolder + "walkbehind2.png") }));
			_room.WalkBehindAreas.Add(new AGSWalkBehindArea (new AGSArea { Mask = maskLoader.Load(_baseFolder + "walkbehind3.png") }));
		
			IObject buildingHotspot = factory.Object.GetHotspot(_baseFolder + "buildingHotspot.png", "Building");
			IObject doorHotspot = factory.Object.GetHotspot(_baseFolder + "doorHotspot.png", "Door");
			IObject windowHotspot = factory.Object.GetHotspot(_baseFolder + "windowHotspot.png", "Window");
			doorHotspot.Z = buildingHotspot.Z - 1;
			windowHotspot.Z = buildingHotspot.Z - 1;
			windowHotspot.Interactions.OnLook.SubscribeToAsync(lookOnWindow);

			_room.Objects.Add(factory.Object.GetHotspot(_baseFolder + "aztecBuildingHotspot.png", "Aztec Building"));
			_room.Objects.Add(buildingHotspot);
			_room.Objects.Add(factory.Object.GetHotspot(_baseFolder + "carHotspot.png", "Car"));
			_room.Objects.Add(doorHotspot);
			_room.Objects.Add(factory.Object.GetHotspot(_baseFolder + "fencesHotspot.png", "Fences"));
			_room.Objects.Add(factory.Object.GetHotspot(_baseFolder + "neonSignHotspot.png", "Neon Sign"));
			_room.Objects.Add(factory.Object.GetHotspot(_baseFolder + "roadHotspot.png", "Road"));
			_room.Objects.Add(factory.Object.GetHotspot(_baseFolder + "sidewalkHotspot.png", "Sidewalk"));
			_room.Objects.Add(factory.Object.GetHotspot(_baseFolder + "skylineHotspot.png", "Skyline"));
			_room.Objects.Add(factory.Object.GetHotspot(_baseFolder + "trashcansHotspot.png", "Trashcans"));
			_room.Objects.Add(windowHotspot);
			addLampPosts(factory);

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
			_player.Character.ChangeRoom(Rooms.TrashcanStreet, 30);
		}

		private void onBeforeFadeIn(object sender, AGSEventArgs args)
		{
			_player.Character.PlaceOnWalkableArea();
		}

		private void addLampPosts(IGameFactory factory)
		{
			PointF parallaxSpeed = new PointF (1.4f, 1f);
			AGSRenderLayer parallaxLayer = new AGSRenderLayer (-50, parallaxSpeed);
			var image = factory.Graphics.LoadImage(_baseFolder + "lampPost.png");
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
			await _player.Character.SayAsync("Hmmm, nobody seems to be home...");
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

