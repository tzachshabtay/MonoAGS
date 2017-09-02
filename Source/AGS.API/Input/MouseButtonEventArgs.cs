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
		/// <param name="mousePosition">The mouse position.</param>
		public MouseButtonEventArgs (IEntity clickedEntity, MouseButton button, MousePosition mousePosition)
		{
            ClickedEntity = clickedEntity;
			Button = button;
            MousePosition = mousePosition;
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
        /// The position of the mouse on the screen at the time the button was clicked.
        /// </summary>
        /// <value>The mouse position.</value>
        public MousePosition MousePosition { get; private set; }
	}
}

