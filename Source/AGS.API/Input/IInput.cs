namespace AGS.API
{
    public interface IInput : IInputEvents
	{
		PointF MousePosition { get; }
		float MouseX { get; }
		float MouseY { get; }
		bool LeftMouseButtonDown { get; }
		bool RightMouseButtonDown { get; }

		IAnimationContainer Cursor { get; set; }
	}
}

