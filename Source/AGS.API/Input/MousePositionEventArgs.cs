namespace AGS.API
{
    /// <summary>
    /// Event arguments for mouse move to indicate the mouse's position.
    /// </summary>
    public class MousePositionEventArgs : AGSEventArgs
	{
		public MousePositionEventArgs (float x, float y)
		{
			X = x;
			Y = y;
		}

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

