namespace AGS.API
{
    /// <summary>
    /// Adds the ability for an entity to check collisions (i.e check whether it collides with a point in the room).
    /// The collision is checked via either the entity's bounding box, or via a pixel perfect collision
    /// if the <see cref="IPixelPerfectComponent"/> exists and enabled.
    /// </summary>
	[RequiredComponent(typeof(IDrawableInfo))]
	[RequiredComponent(typeof(IAnimationContainer))]
    [RequiredComponent(typeof(IScaleComponent))]
    [RequiredComponent(typeof(IPixelPerfectComponent))]
	public interface ICollider : IComponent
	{
        /// <summary>
        /// Gets or sets the bounding box which surrounds the entity.
        /// The bounding box is set by the engine, and should not be set by the user.
        /// </summary>
        /// <value>The bounding box.</value>
		AGSBoundingBox BoundingBox { get; set; } //todo: find a way to remove the setter (only the engine should use the setter)

        /// <summary>
        /// Gets the center point of the entity.
        /// </summary>
        /// <value>The center point.</value>
		PointF? CenterPoint { get; }

        /// <summary>
        /// Does the entity collide with point (x,y)?
        /// The (x,y) is expected in room-coordinates after adjusting for the viewport.
        /// The collision is checked via either the entity's bounding box, or via a pixel perfect collision
        /// if the <see cref="IPixelPerfectComponent"/> exists and enabled.
        /// </summary>
        /// <returns><c>true</c>, if it collides with the point, <c>false</c> otherwise.</returns>
        /// <param name="x">The x coordinate.</param>
        /// <param name="y">The y coordinate.</param>
		bool CollidesWith(float x, float y);
	}
}

