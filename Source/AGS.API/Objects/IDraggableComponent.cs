namespace AGS.API
{
    /// <summary>
    /// Adds the ability for an entity to be dragged on the screen with the mouse/touch.
    /// </summary>
    [RequiredComponent(typeof(ITranslateComponent))]
    [RequiredComponent(typeof(IDrawableInfoComponent), false)]
    [RequiredComponent(typeof(IUIEvents))]
    public interface IDraggableComponent : IComponent
    {
        /// <summary>
        /// Gets or sets a value indicating whether this entity can be dragged now.
        /// </summary>
        /// <value><c>true</c> if is drag enabled; otherwise, <c>false</c>.</value>
        bool IsDragEnabled { get; set; }

        /// <summary>
        /// Gets a value indicating whether this entity is currently dragged.
        /// </summary>
        /// <value><c>true</c> if is currently dragged; otherwise, <c>false</c>.</value>
        bool IsCurrentlyDragged { get; }

        /// <summary>
        /// Allows to limit the dragging to a minimum x (as room coordinates after adjusting for the viewport).
        /// </summary>
        /// <value>The drag minimum x.</value>
        float? DragMinX { get; set; }

        /// <summary>
        /// Allows to limit the dragging to a maximum x (as room coordinates after adjusting for the viewport).
        /// </summary>
        /// <value>The drag maximum x.</value>
        float? DragMaxX { get; set; }

        /// <summary>
        /// Allows to limit the dragging to a minimum y (as room coordinates after adjusting for the viewport).
        /// </summary>
        /// <value>The drag minimum y.</value>
        float? DragMinY { get; set; }

        /// <summary>
        /// Allows to limit the dragging to a maximum y (as room coordinates after adjusting for the viewport).
        /// </summary>
        /// <value>The drag maximum y.</value>
        float? DragMaxY { get; set; }

        /// <summary>
        /// An event that fires when dragging the entity starts. The x & y properties
        /// of the entity are sent as the event arguments.
        /// </summary>
        /// <value>The event.</value>
        IBlockingEvent<(float dragStartX, float dragStartY)> OnDragStart { get; }
    }
}