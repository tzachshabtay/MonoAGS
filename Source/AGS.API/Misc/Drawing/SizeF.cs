using System;

namespace AGS.API
{
	public struct SizeF
	{
		private readonly float _width, _height;

		public SizeF(float width, float height)
		{
			_width = width;
			_height = height;
		}

		public float Width { get { return _width; } }
		public float Height { get { return _height; } }
	}
}

