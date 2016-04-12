using System;

namespace AGS.API
{
	public struct SizeF
	{
		public SizeF(float width, float height)
		{
			Width = width;
			Height = height;
		}

		public float Width { get; private set; }
		public float Height { get; private set; }
	}
}

