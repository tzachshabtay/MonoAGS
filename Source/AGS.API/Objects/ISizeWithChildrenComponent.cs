namespace AGS.API
{
    /// <summary>
    /// Allows to query the size of the entity combined with all its children (i.e the size
    /// of the minimal bounding box that surrounds them all).
    /// </summary>
    [RequiredComponent(typeof(ITranslateComponent))]
    [RequiredComponent(typeof(IScaleComponent))]
    [RequiredComponent(typeof(IInObjectTree))]
    public interface ISizeWithChildrenComponent : IComponent
    {
        /// <summary>
        /// Gets the size with children.
        /// </summary>
        /// <value>The size with children.</value>
        SizeF SizeWithChildren { get; }

        /// <summary>
        /// An event which fires whenever the size with children changes.
        /// </summary>
        /// <value>The on size with children changed.</value>
        IEvent<object> OnSizeWithChildrenChanged { get; }
    }
}
