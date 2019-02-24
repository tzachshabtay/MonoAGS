namespace AGS.API
{
    /// <summary>
    /// Mouse click/double-click event arguments.
    /// </summary>
    public class MouseClickEventArgs : MouseButtonEventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="T:AGS.API.MouseClickEventArgs"/> class.
        /// </summary>
        /// <param name="clickedEntity">Clicked entity.</param>
        /// <param name="button">Button.</param>
        /// <param name="mousePosition">Mouse position.</param>
        /// <param name="clickTimeInMilliseconds">Click time in milliseconds.</param>
        public MouseClickEventArgs(IEntity clickedEntity, MouseButton button, MousePosition mousePosition, long clickTimeInMilliseconds)
            : base(clickedEntity, button, mousePosition)
        {
            ClickTimeInMilliseconds = clickTimeInMilliseconds;
        }

        /// <summary>
        /// How much time has passed between the mouse button down and up states.
        /// </summary>
        /// <value>The click time in milliseconds.</value>
        public long ClickTimeInMilliseconds { get; }
    }
}