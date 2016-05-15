using System;

namespace AGS.API
{
	public struct Size
	{
		private readonly int _width, _height;

		public Size(int width, int height)
		{
			_width = width;
			_height = height;
		}

		public int Width { get { return _width; } }
		public int Height { get { return _height; } }

		public override string ToString()
		{
			return string.Format("[Size: Width={0}, Height={1}]", Width, Height);
		}
	}
}

