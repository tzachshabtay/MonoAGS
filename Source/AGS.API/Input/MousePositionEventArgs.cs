namespace AGS.API
{
    /// <summary>
    /// Event arguments for mouse move to indicate the mouse's position.
    /// </summary>
    public class MousePositionEventArgs
	{
        public MousePositionEventArgs (MousePosition position)
		{
            MousePosition = position;
		}

        /// <summary>
        /// The position of the mouse on the screen.
        /// </summary>
        /// <value>The mouse position.</value>
        public MousePosition MousePosition { get; private set; }
	}
}

