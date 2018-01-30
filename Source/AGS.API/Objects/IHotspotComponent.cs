namespace AGS.API
{
    /// <summary>
    /// Allows an entity to act as a hotspot (i.e can be interacted with).
    /// </summary>
	public interface IHotspotComponent : IComponent
	{
        /// <summary>
        /// Allows subscribing to interaction events for this entity.
        /// </summary>
        /// <seealso cref="IInteractions"/>
        /// <value>The interactions.</value>
		IInteractions Interactions { get; }

        /// <summary>
        /// Gets or sets an optional walk point for the entity which might be used before interacting with the
        /// entity, depending on how the <see cref="IApproachComponent"/> is configured.
        /// </summary>
        /// <seealso cref="IApproachStyle"/>
        /// <value>The walk point.</value>
		PointF? WalkPoint { get; set; }
	}
}

