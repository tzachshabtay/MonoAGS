using System;
using AGS.API;
using AGS.Engine;
using System.Drawing;

namespace DemoGame
{
	public class BrokenCurbStreet
	{
		private readonly IRoom _room;
		private readonly IPlayer _player;
		private const string _baseFolder = "../../Assets/Rooms/BrokenCurbStreet/";

		public BrokenCurbStreet(IPlayer player, IViewport viewport, IGameEvents gameEvents)
		{
			_player = player;
			AGSEdges edges = new AGSEdges (new AGSEdge { Value = 20f },
				new AGSEdge { Value = 310f }, new AGSEdge { Value = 190f },
				new AGSEdge { Value = 10f });
			edges.Left.OnEdgeCrossed.Subscribe(onLeftEdgeCrossed);
			_room = new AGSRoom ("Broken Curb Street", player, viewport, edges, gameEvents);
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
			_room.WalkableAreas.Add(new AGSArea { Mask = maskLoader.Load(_baseFolder + "walkable3.png"), Enabled = false });
			_room.WalkableAreas.Add(new AGSArea { Mask = maskLoader.Load(_baseFolder + "walkable4.png") });

			_room.WalkBehindAreas.Add(new AGSWalkBehindArea (new AGSArea { Mask = maskLoader.Load(_baseFolder + "walkbehind1.png") }));
			_room.WalkBehindAreas.Add(new AGSWalkBehindArea (new AGSArea { Mask = maskLoader.Load(_baseFolder + "walkbehind2.png") }));

			IObject wallHotspot = factory.GetHotspot(_baseFolder + "wallHotspot.png", "Wall");
			IObject graffitiHotspot = factory.GetHotspot(_baseFolder + "graffitiHotspot.png", "Graffiti");
			IObject doorHotspot = factory.GetHotspot(_baseFolder + "doorHotspot.png", "Door");
			doorHotspot.Z = wallHotspot.Z - 1;
			graffitiHotspot.Z = wallHotspot.Z - 1;

			_room.Objects.Add(factory.GetHotspot(_baseFolder + "barrierHotspot.png", "Barrier"));
			_room.Objects.Add(factory.GetHotspot(_baseFolder + "bloodySidewalkHotspot.png", "Bloody Sidewalk"));
			_room.Objects.Add(factory.GetHotspot(_baseFolder + "brokenCurbHotspot.png", "Broken Curb"));
			_room.Objects.Add(factory.GetHotspot(_baseFolder + "curbHotspot.png", "Curb"));
			_room.Objects.Add(doorHotspot);
			_room.Objects.Add(graffitiHotspot);
			_room.Objects.Add(factory.GetHotspot(_baseFolder + "manholeHotspot.png", "Manhole"));
			_room.Objects.Add(factory.GetHotspot(_baseFolder + "roadHotspot.png", "Road"));
			_room.Objects.Add(factory.GetHotspot(_baseFolder + "sidewalkHotspot.png", "Sidewalk"));
			_room.Objects.Add(factory.GetHotspot(_baseFolder + "slimeHotspot.png", "Slime"));
			_room.Objects.Add(wallHotspot);



			return _room;
		}

		private void onLeftEdgeCrossed(object sender, EventArgs args)
		{
			_player.Character.ChangeRoom(Rooms.EmptyStreet, 310);
		}
	}
}

