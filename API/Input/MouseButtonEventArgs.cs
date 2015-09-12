using System;

namespace API
{
	public class MouseButtonEventArgs : EventArgs
	{
		public MouseButtonEventArgs (MouseButton button, float x, float y)
		{
			Button = button;
			X = x;
			Y = y;
		}

		public MouseButton Button { get; private set; }

		public float X { get; private set; }
		public float Y { get; private set; }
	}
}

