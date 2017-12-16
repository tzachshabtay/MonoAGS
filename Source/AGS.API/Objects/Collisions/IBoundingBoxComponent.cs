namespace AGS.API
{
    /// <summary>
    /// A component that calculates the bounding box of the entity.
    /// This is for both the bounding box used to render the entity, and for collision checks.
    /// </summary>
    [RequiredComponent(typeof(IModelMatrixComponent))]
    [RequiredComponent(typeof(IImageComponent))]
    [RequiredComponent(typeof(IAnimationComponent))]
    [RequiredComponent(typeof(IDrawableInfoComponent))]
    [RequiredComponent(typeof(IHasRoomComponent))]
    [RequiredComponent(typeof(ICropSelfComponent), false)]
    public interface IBoundingBoxComponent : IComponent
    {
        /// <summary>
        /// Gets or sets the bounding box which surrounds the entity.
        /// The bounding box is set by the engine, and should not be set by the user.
        /// </summary>
        /// <value>The bounding boxes.</value>
        AGSBoundingBoxes GetBoundingBoxes(IViewport viewport);

        /// <summary>
        /// An event which fires whenever the bounding boxes for the entity change.
        /// </summary>
        /// <value>The on bounding boxes changed event.</value>
        IBlockingEvent OnBoundingBoxesChanged { get; }

        /// <summary>
        /// Allows locking the component from changing (to allow for changing multiple components "at once").
        /// </summary>
        /// <value>The lock step.</value>
        ILockStep BoundingBoxLockStep { get; }

	}
}
