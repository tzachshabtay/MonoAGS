namespace AGS.API
{
    /// <summary>
    /// It might be that not all of the room is shown on the screen at once (for example, a scrolling room). 
    /// A viewport to the room instructs the engine on what parts of the room to show.
    /// </summary>
    /// <seealso cref="IRoom"/>
    public interface IViewport
	{
        /// <summary>
        /// The left location of the room from which to show the screen.
        /// </summary>
        /// <value>The x.</value>
		float X { get; set; }

        /// <summary>
        /// The bottom location for the room from which to show the screen.
        /// </summary>
        /// <value>The y.</value>
		float Y { get; set; }

        /// <summary>
        /// The horizontal zoom factor of the room.
        /// </summary>
        /// <value>The scale x.</value>
		float ScaleX { get; set; }

        /// <summary>
        /// The vertical zoom factor of the room.
        /// </summary>
        /// <value>The scale y.</value>
		float ScaleY { get; set; }

        /// <summary>
        /// The rotation angle (in degrees) of the viewport.
        /// </summary>
        /// <value>The angle.</value>
		float Angle { get; set; }

        /// <summary>
        /// Gets or sets the camera which automatically manipulates the viewport to follow a target (usually the player).
        /// </summary>
        /// <value>The camera.</value>
		ICamera Camera { get; set; }

        /// <summary>
        /// An event which fires when the position of the viewport has changed.
        /// </summary>
        /// <value>The on position changed.</value>
        IEvent<object> OnPositionChanged { get; }

        /// <summary>
        /// An event which fires when the size of the viewport has changed.
        /// </summary>
        /// <value>The on scale changed.</value>
        IEvent<object> OnScaleChanged { get; }

        /// <summary>
        /// An event which fires when the angle of the viewport has changed.
        /// </summary>
        /// <value>The on angle changed.</value>
        IEvent<object> OnAngleChanged { get; }
	}
}

