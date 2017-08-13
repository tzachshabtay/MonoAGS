namespace AGS.API
{
    /// <summary>
    /// Allows to query the size of the entity combined with all its children (i.e the size
    /// of the minimal bounding box that surrounds them all).
    /// </summary>
    [RequiredComponent(typeof(IBoundingBoxComponent))]
    [RequiredComponent(typeof(IInObjectTree))]
    public interface IBoundingBoxWithChildrenComponent : IComponent
    {
        /// <summary>
        /// Gets the bounding box which wraps around the entity and all its children.
        /// </summary>
        /// <value>The size with children.</value>
        AGSBoundingBox BoundingBoxWithChildren { get; }

		/// <summary>
		/// Gets the bounding box which wraps around the entity and all its children
        /// before any of the children was cropped (for entities which has the <see cref="ICropSelfComponent"/>
        /// attached to them. 
		/// </summary>
		/// <value>The pre crop bounding box with children.</value>
		AGSBoundingBox PreCropBoundingBoxWithChildren { get; }

        /// <summary>
        /// An event which fires whenever the bounding box with children changes.
        /// </summary>
        /// <value>The on bounding box with children changed.</value>
        IEvent<object> OnBoundingBoxWithChildrenChanged { get; }
    }
}
