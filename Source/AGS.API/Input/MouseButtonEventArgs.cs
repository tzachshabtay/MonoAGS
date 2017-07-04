namespace AGS.API
{
    /// <summary>
    /// Mouse button down/up event arguments.
    /// </summary>
    public class MouseButtonEventArgs
	{
        /// <summary>
        /// Initializes a new instance of the <see cref="T:AGS.API.MouseButtonEventArgs"/> class.
        /// </summary>
        /// <param name="clickedEntity">The entity which was clicked on.</param>
        /// <param name="button">Button.</param>
        /// <param name="x">The x coordinate.</param>
        /// <param name="y">The y coordinate.</param>
		public MouseButtonEventArgs (IEntity clickedEntity, MouseButton button, float x, float y)
		{
            ClickedEntity = clickedEntity;
			Button = button;
			X = x;
			Y = y;
		}

        /// <summary>
        /// The entity which was clicked on.
        /// </summary>
        /// <value>The clicked entity.</value>
        public IEntity ClickedEntity { get; private set; }

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

