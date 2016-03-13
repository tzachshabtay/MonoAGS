namespace AGS.API
{
    public class MousePositionEventArgs : AGSEventArgs
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

