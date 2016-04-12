using System;

namespace AGS.API
{
	public struct Size
	{
		public Size(int width, int height)
		{
			Width = width;
			Height = height;
		}

		public int Width { get; private set; }
		public int Height { get; private set; }
	}
}

