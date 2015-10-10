using System;
using AGS.API;
using AGS.Engine;
using System.Threading.Tasks;
using System.Drawing;

namespace DemoGame
{
	public class EmptyStreet
	{
		private IRoom _room;
		private readonly IPlayer _player;
		private IObject _bottle;

		private const string _baseFolder = "../../Assets/Rooms/EmptyStreet/";

		public EmptyStreet(IPlayer player)
		{
			_player = player;
		}
			
		public IRoom Load(IGameFactory factory)
		{
			ILoadImageConfig loadConfig = new AGSLoadImageConfig
			{
				TransparentColorSamplePoint = new Point(0, 0)
			};
			_room = factory.Room.GetRoom("Empty Street", 20f, 310f, 190f, 10f);
			_room.Edges.Left.OnEdgeCrossed.Subscribe(onLeftEdgeCrossed);
			_room.Edges.Right.OnEdgeCrossed.Subscribe(onRightEdgeCrossed);
			_room.Events.OnBeforeFadeIn.Subscribe(onBeforeFadeIn);

			IObject bg = factory.Object.GetObject();
			bg.Image = factory.Graphics.LoadImage(_baseFolder + "bg.png");
			_room.Background = bg;

			AGSMaskLoader maskLoader = new AGSMaskLoader (factory);
			_room.WalkableAreas.Add(new AGSArea { Mask = maskLoader.Load(_baseFolder + "walkable1.png") });
			_room.WalkableAreas.Add(new AGSArea { Mask = maskLoader.Load(_baseFolder + "walkable2.png") });
			_room.ScalingAreas.Add(AGSScalingArea.Create(_room.WalkableAreas[0], 0.50f, 0.75f));
			_room.ScalingAreas.Add(AGSScalingArea.Create(_room.WalkableAreas[1], 0.75f, 0.90f));

			IObject bottleHotspot = factory.Object.GetHotspot(_baseFolder + "BottleHotspot.png", "Bottle");
			bottleHotspot.WalkPoint = new AGSPoint (140f, 50f);
			_room.Objects.Add(factory.Object.GetHotspot(_baseFolder + "CurbHotspot.png", "Curb"));
			_room.Objects.Add(bottleHotspot);
			_room.Objects.Add(factory.Object.GetHotspot(_baseFolder + "GapHotspot.png", "Gap", new[]{"It's a gap!", "I wonder what's in there!"}));

			_bottle = factory.Object.GetObject();
			_bottle.Image = factory.Graphics.LoadImage(_baseFolder + "bottle.bmp", loadConfig: loadConfig);
			_bottle.WalkPoint = bottleHotspot.WalkPoint;
			_bottle.X = 185f;
			_bottle.Y = 85f;
			_bottle.Hotspot = "Bottle";
			_bottle.Interactions.OnInteract.Subscribe(onBottleInteract);
			_room.Objects.Add(_bottle);

			return _room;
		}

		private void onBottleInteract(object sender, AGSEventArgs args)
		{
			_bottle.ChangeRoom(null);
			_player.Character.Inventory.Items.Add(InventoryItems.Bottle);
		}

		private void onLeftEdgeCrossed(object sender, AGSEventArgs args)
		{
			_player.Character.ChangeRoom(Rooms.TrashcanStreet, 310);
		}

		private void onRightEdgeCrossed(object sender, AGSEventArgs args)
		{
			_player.Character.ChangeRoom(Rooms.BrokenCurbStreet, 30);
		}

		private void onBeforeFadeIn(object sender, AGSEventArgs args)
		{
			_player.Character.PlaceOnWalkableArea();
		}
	}
}

