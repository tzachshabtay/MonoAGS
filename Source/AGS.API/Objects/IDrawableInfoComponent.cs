namespace AGS.API
{
    /// <summary>
    /// Adds the ability for an entity to configure various aspects of how it is rendered.
    /// </summary>
	public interface IDrawableInfoComponent : IComponent
	{
        /// <summary>
        /// Gets or sets the render layer associated with the entity.
        /// </summary>
        /// <seealso cref="IRenderLayer"/>
        /// <value>The render layer.</value>
		IRenderLayer RenderLayer { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this entity's position is not affected by the viewport.
        /// If the viewport is ignored, then the entity will remain on the same place on the screen even if the camera
        /// moves around. Therefore, it's on by default for GUI controls which usually are not part of the game world itself.
        /// </summary>
        /// <value><c>true</c> if ignore viewport; otherwise, <c>false</c>.</value>
		bool IgnoreViewport { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this entity's scaling is not affected by scaling areas in the room.
        /// If the scaling areas are ignored, then the entity will keep its scale even if it's "sitting" in a scaling areas.
        /// Therefore, it's on by default for GUI controls which usually are not part of the game world itself.
        /// </summary>
        /// <value><c>true</c> if ignore scaling area; otherwise, <c>false</c>.</value>
		bool IgnoreScalingArea { get; set; }
	}
}

