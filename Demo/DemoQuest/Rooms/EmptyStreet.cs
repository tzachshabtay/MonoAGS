using System;
using AGS.API;
using AGS.Engine;
using System.Threading.Tasks;
using System.Linq;

namespace DemoGame
{
	public class EmptyStreet
	{
		private IRoom _room;
		private IPlayer _player;
		private IObject _bottle;
		private IGame _game;

		private const string _baseFolder = "../../Assets/Rooms/EmptyStreet/";
		private const string _roomId = "Empty Street";
		private const string _bottleId = "Bottle (object)";

		public EmptyStreet(IPlayer player)
		{
			_player = player;
		}
			
		public IRoom Load(IGame game)
		{
			_game = game;
			IGameFactory factory = game.Factory;
			ILoadImageConfig loadConfig = new AGSLoadImageConfig
			{
				TransparentColorSamplePoint = new AGS.API.Point(0, 0)
			};
			_room = factory.Room.GetRoom(_roomId, 20f, 310f, 190f, 10f);
			_game.Events.OnSavedGameLoad.Subscribe((sender, e) => onSavedGameLoaded());

			IObject bg = factory.Object.GetObject("Empty Street BG");
			bg.Image = factory.Graphics.LoadImage(_baseFolder + "bg.png");
			_room.Background = bg;

			AGSMaskLoader maskLoader = new AGSMaskLoader (factory);
			_room.WalkableAreas.Add(new AGSArea { Mask = maskLoader.Load(_baseFolder + "walkable1.png") });
			_room.WalkableAreas.Add(new AGSArea { Mask = maskLoader.Load(_baseFolder + "walkable2.png") });
			_room.ScalingAreas.Add(AGSScalingArea.Create(_room.WalkableAreas[0], 0.50f, 0.75f));
			_room.ScalingAreas.Add(AGSScalingArea.Create(_room.WalkableAreas[1], 0.75f, 0.90f));

			IObject bottleHotspot = factory.Object.GetHotspot(_baseFolder + "BottleHotspot.png", "Bottle");
			bottleHotspot.WalkPoint = new AGS.API.PointF (140f, 50f);
			_room.Objects.Add(factory.Object.GetHotspot(_baseFolder + "CurbHotspot.png", "Curb"));
			_room.Objects.Add(bottleHotspot);
			_room.Objects.Add(factory.Object.GetHotspot(_baseFolder + "GapHotspot.png", "Gap", new[]{"It's a gap!", "I wonder what's in there!"}));

			_bottle = factory.Object.GetObject(_bottleId);
			_bottle.Image = factory.Graphics.LoadImage(_baseFolder + "bottle.bmp", loadConfig: loadConfig);
			_bottle.WalkPoint = bottleHotspot.WalkPoint;
			_bottle.X = 185f;
			_bottle.Y = 85f;
			_bottle.Hotspot = "Bottle";
			_room.Objects.Add(_bottle);

			subscribeEvents();

			return _room;
		}

		private void onSavedGameLoaded()
		{
			_player = _game.State.Player;
			_room = Rooms.Find(_game.State, _room);
			_bottle = _room.Find<IObject>(_bottleId);
			subscribeEvents();
		}

		private void subscribeEvents()
		{
			_room.Edges.Left.OnEdgeCrossed.Subscribe(onLeftEdgeCrossed);
			_room.Edges.Right.OnEdgeCrossed.Subscribe(onRightEdgeCrossed);
			_room.Events.OnBeforeFadeIn.Subscribe(onBeforeFadeIn);
			if (_bottle != null) _bottle.Interactions.OnInteract.Subscribe(onBottleInteract);
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

