namespace AGS.API
{
    /// <summary>
    /// Mouse buttons.
    /// </summary>
    public enum MouseButton
	{
        /// <summary>
        /// Left mouse button
        /// </summary>
		Left,
        /// <summary>
        /// Right mouse button
        /// </summary>
		Right,
        /// <summary>
        /// Middle mouse button
        /// </summary>
		Middle,
	}
	
    /// <summary>
    /// A collection of input events.
    /// </summary>
	public interface IInputEvents
	{
        /// <summary>
        /// An event which is triggered when the user presses down one of the mouse buttons.
        /// The event arguments specify which mouse button was pressed and where on the screen.
        /// </summary>
        /// <value>The event.</value>
		IEvent<MouseButtonEventArgs> MouseDown { get; }

        /// <summary>
        /// An event which is triggered when the user releases one of the mouse buttons (which was previously pressed).
        /// The event arguments specify which mouse button was released and where on the screen.
        /// </summary>
        /// <value>The event.</value>
		IEvent<MouseButtonEventArgs> MouseUp { get; }

        /// <summary>
        /// An event which is triggered every time the user moves the mouse.
        /// The event arguments specify the location that the mouse moved to.
        /// </summary>
        /// <value>The event.</value>
		IEvent<MousePositionEventArgs> MouseMove { get; }

        /// <summary>
        /// An event which is triggered when the user presses down one of the keys on the keyboard.
        /// The event argumens specify which key was pressed.
        /// </summary>
        /// <value>The event.</value>
		IEvent<KeyboardEventArgs> KeyDown { get; }

        /// <summary>
        /// An event which is triggered when the user releases one of the keys on the keyboard (which was previously released).
        /// The event arguments specify which key was released.
        /// </summary>
        /// <value>The event.</value>
		IEvent<KeyboardEventArgs> KeyUp { get; }
	}
}

