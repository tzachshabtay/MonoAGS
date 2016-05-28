using System;
using AGS.API;
using AGS.Engine;

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

			AGSMaskLoader maskLoader = new AGSMaskLoader (factory);
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
	}
}

