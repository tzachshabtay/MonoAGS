using System;

namespace AGS.API
{
	public struct Point
	{
		public Point(int x, int y)
		{
			X = x;
			Y = y;
		}

		public int X { get; private set; }
		public int Y { get; private set; }
	}
}

