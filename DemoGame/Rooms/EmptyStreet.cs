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

		private const string _baseFolder = "../../Assets/Rooms/EmptyStreet/";

		public EmptyStreet(IPlayer player)
		{
			_player = player;
		}
			
		public IRoom Load(IGameFactory factory)
		{
			_room = factory.GetRoom("Empty Street", 20f, 310f, 190f, 10f);
			_room.Edges.Left.OnEdgeCrossed.Subscribe(onLeftEdgeCrossed);
			_room.Edges.Right.OnEdgeCrossed.Subscribe(onRightEdgeCrossed);
			_room.Events.OnBeforeFadeIn.Subscribe(onBeforeFadeIn);

			IObject bg = factory.GetObject();
			bg.Image = factory.Graphics.LoadImage(_baseFolder + "bg.png");
			_room.Background = bg;

			AGSMaskLoader maskLoader = new AGSMaskLoader (factory);
			_room.WalkableAreas.Add(new AGSArea { Mask = maskLoader.Load(_baseFolder + "walkable1.png") });
			_room.WalkableAreas.Add(new AGSArea { Mask = maskLoader.Load(_baseFolder + "walkable2.png") });
			_room.ScalingAreas.Add(AGSScalingArea.Create(_room.WalkableAreas[0], 0.50f, 0.75f));
			_room.ScalingAreas.Add(AGSScalingArea.Create(_room.WalkableAreas[1], 0.75f, 0.90f));

			_room.Objects.Add(factory.GetHotspot(_baseFolder + "CurbHotspot.png", "Curb"));
			_room.Objects.Add(factory.GetHotspot(_baseFolder + "BottleHotspot.png", "Bottle"));
			_room.Objects.Add(factory.GetHotspot(_baseFolder + "GapHotspot.png", "Gap", new[]{"It's a gap!", "I wonder what's in there!"}));

			return _room;
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

