namespace AGS.API
{
    /// <summary>
    /// This component gives the ability to attach scrollbars to an entity, which will then
    /// scroll the content (i.e the children) of the entity.
    /// </summary>
    [RequiredComponent(typeof(ICropChildrenComponent))]
    public interface IScrollingComponent : IComponent
    {
        /// <summary>
        /// Gets or sets the vertical scroll bar.
        /// </summary>
        /// <value>The vertical scroll bar.</value>
        ISlider VerticalScrollBar { get; set; }

        /// <summary>
        /// Gets or sets the horizontal scroll bar.
        /// </summary>
        /// <value>The horizontal scroll bar.</value>
        ISlider HorizontalScrollBar { get; set; }
    }
}
