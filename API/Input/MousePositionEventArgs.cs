using System;

namespace API
{
	public class MousePositionEventArgs : EventArgs
	{
		public MousePositionEventArgs (float x, float y)
		{
			X = x;
			Y = y;
		}

		public float X { get; private set; }
		public float Y { get; private set; }
	}
}

