using System;
using AGS.API;
using AGS.Engine;
using System.Drawing;

namespace DemoGame
{
	public class DarsStreet
	{
		private readonly IRoom _room;
		private readonly IPlayer _player;
		private const string _baseFolder = "../../Assets/Rooms/DarsStreet/";

		public DarsStreet(IPlayer player, IViewport viewport, IGameEvents gameEvents)
		{
			_player = player;
			AGSEdges edges = new AGSEdges (new AGSEdge { Value = 20f },
				new AGSEdge { Value = 490f }, new AGSEdge { Value = 190f },
				new AGSEdge { Value = 10f });
			edges.Right.OnEdgeCrossed.Subscribe(onRightEdgeCrossed);
			_room = new AGSRoom ("Dars Street", player, viewport, edges, gameEvents);
			viewport.Follower = new AGSViewportFollower { Target = () => player.Character };
		}

		public IRoom Load(IGameFactory factory)
		{
			IObject bg = factory.GetObject();
			bg.Image = factory.Graphics.LoadImage(_baseFolder + "bg.png");
			_room.Background = bg;

			AGSMaskLoader maskLoader = new AGSMaskLoader (factory);
			_room.WalkableAreas.Add(new AGSArea { Mask = maskLoader.Load(_baseFolder + "walkable1.png") });
			_room.WalkableAreas.Add(new AGSArea { Mask = maskLoader.Load(_baseFolder + "walkable2.png") });

			_room.WalkBehindAreas.Add(new AGSWalkBehindArea (new AGSArea { Mask = maskLoader.Load(_baseFolder + "walkbehind1.png") }));
			_room.WalkBehindAreas.Add(new AGSWalkBehindArea (new AGSArea { Mask = maskLoader.Load(_baseFolder + "walkbehind2.png") }));
			_room.WalkBehindAreas.Add(new AGSWalkBehindArea (new AGSArea { Mask = maskLoader.Load(_baseFolder + "walkbehind3.png") }));
		
			IObject buildingHotspot = factory.GetHotspot(_baseFolder + "buildingHotspot.png", "Building");
			IObject doorHotspot = factory.GetHotspot(_baseFolder + "doorHotspot.png", "Door");
			IObject windowHotspot = factory.GetHotspot(_baseFolder + "windowHotspot.png", "Window");
			doorHotspot.Z = buildingHotspot.Z - 1;
			windowHotspot.Z = buildingHotspot.Z - 1;
			_room.Objects.Add(factory.GetHotspot(_baseFolder + "aztecBuildingHotspot.png", "Aztec Building"));
			_room.Objects.Add(buildingHotspot);
			_room.Objects.Add(factory.GetHotspot(_baseFolder + "carHotspot.png", "Car"));
			_room.Objects.Add(doorHotspot);
			_room.Objects.Add(factory.GetHotspot(_baseFolder + "fencesHotspot.png", "Fences"));
			_room.Objects.Add(factory.GetHotspot(_baseFolder + "neonSignHotspot.png", "Neon Sign"));
			_room.Objects.Add(factory.GetHotspot(_baseFolder + "roadHotspot.png", "Road"));
			_room.Objects.Add(factory.GetHotspot(_baseFolder + "sidewalkHotspot.png", "Sidewalk"));
			_room.Objects.Add(factory.GetHotspot(_baseFolder + "skylineHotspot.png", "Skyline"));
			_room.Objects.Add(factory.GetHotspot(_baseFolder + "trashcansHotspot.png", "Trashcans"));
			_room.Objects.Add(windowHotspot);

			return _room;
		}

		private void onRightEdgeCrossed(object sender, EventArgs args)
		{
			_player.Character.ChangeRoom(Rooms.TrashcanStreet, 30);
		}
	}
}

