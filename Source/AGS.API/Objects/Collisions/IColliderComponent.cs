namespace AGS.API
{
    /// <summary>
    /// Adds the ability for an entity to check collisions (i.e check whether it collides with a point in the room).
    /// The collision is checked via either the entity's bounding box, or via a pixel perfect collision
    /// if the <see cref="IPixelPerfectComponent"/> exists and enabled.
    /// </summary>
	[RequiredComponent(typeof(IDrawableInfoComponent))]
	[RequiredComponent(typeof(IAnimationComponent))]
    [RequiredComponent(typeof(IScaleComponent))]
    [RequiredComponent(typeof(IPixelPerfectComponent))]
	public interface IColliderComponent : IComponent
	{
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
        /// <param name="viewport">The viewport to check the collisions against</param>
        bool CollidesWith(float x, float y, IViewport viewport);
	}
}

