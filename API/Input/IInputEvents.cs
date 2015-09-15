using System;

namespace AGS.API
{
	public enum MouseButton
	{
		Left,
		Right,
		Middle,
	}
		
	public interface IInputEvents
	{
		IEvent<MouseButtonEventArgs> MouseDown { get; }
		IEvent<MouseButtonEventArgs> MouseUp { get; }
		IEvent<MousePositionEventArgs> MouseMove { get; }

		IEvent<KeyboardEventArgs> KeyDown { get; }
		IEvent<KeyboardEventArgs> KeyUp { get; }
	}
}

