using System;

namespace AGS.API
{
	public interface IInput : IInputEvents
	{
		IPoint MousePosition { get; }
		float MouseX { get; }
		float MouseY { get; }
		IAnimationContainer Cursor { get; set; }
	}
}

