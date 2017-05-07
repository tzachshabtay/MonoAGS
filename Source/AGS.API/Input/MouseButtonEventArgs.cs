namespace AGS.API
{
    /// <summary>
    /// Mouse button down/up event arguments.
    /// </summary>
    public class MouseButtonEventArgs : AGSEventArgs
	{
        /// <summary>
        /// Initializes a new instance of the <see cref="T:AGS.API.MouseButtonEventArgs"/> class.
        /// </summary>
        /// <param name="button">Button.</param>
        /// <param name="x">The x coordinate.</param>
        /// <param name="y">The y coordinate.</param>
		public MouseButtonEventArgs (MouseButton button, float x, float y)
		{
			Button = button;
			X = x;
			Y = y;
		}

        /// <summary>
        /// Which mouse button is this event relates to.
        /// </summary>
        /// <value>The button.</value>
		public MouseButton Button { get; private set; }

        /// <summary>
        /// Gets the mouse's x position in the room's co-ordinates (based on the game's virtual resolution), 
        /// after adjusting for the viewport.
        /// </summary>
        /// <value>The mouse x.</value>
		public float X { get; private set; }

        /// <summary>
        /// Gets the mouse's y position in the room's co-ordinates (based on the game's virtual resolution), 
        /// after adjusting for the viewport.
        /// </summary>
        /// <value>The mouse y.</value>
		public float Y { get; private set; }
	}
}

