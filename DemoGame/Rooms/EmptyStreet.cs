using System;
using API;
using Engine;
using System.Threading.Tasks;
using System.Drawing;

namespace DemoGame
{
	public class EmptyStreet
	{
		private IRoom _room;
		private const string _baseFolder = "../../Assets/Rooms/EmptyStreet/";

		public EmptyStreet(IPlayer player, IViewport viewport)
		{
			_room = new AGSRoom ("Empty Street", player, viewport);
		}


		public IRoom Load(IGraphicsFactory factory)
		{
			AGSObject bg = new AGSObject (new AGSSprite ());
			bg.Image = factory.LoadImage(_baseFolder + "bg.png");
			_room.Background = bg;

			Bitmap debugBitmap;
			_room.WalkableAreas.Add(new AGSArea { Mask = GraphicsUtils.LoadMask(_baseFolder + "walkable1.png", out debugBitmap) });
			_room.WalkableAreas.Add(new AGSArea { Mask = GraphicsUtils.LoadMask(_baseFolder + "walkable2.png", out debugBitmap) });

			return _room;
		}
	}
}

