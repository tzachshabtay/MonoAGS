using System;

namespace AGS.API
{
	public struct Rectangle
	{
		private readonly int _x, _y, _width, _height;

		public Rectangle(int x, int y, int width, int height)
		{
			_x = x;
			_y = y;
			_width = width;
			_height = height;
		}

		public int X { get { return _x; } }
		public int Y { get { return _y; } }
		public int Width { get { return _width; } }
		public int Height { get { return _height; } }
	}
}

