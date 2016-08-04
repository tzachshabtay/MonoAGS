namespace AGS.API
{
    /// <summary>
    /// Allows setting a special cursor that will be shown when the mouse is hovering the entity.
    /// This can be useful for having special "Exit" cursors for doors (for example).
    /// </summary>
    public interface IHasCursorComponent : IComponent
    {
        /// <summary>
        /// Gets or sets the special cursor the will be shown on hover.
        /// </summary>
        /// <value>The special cursor.</value>
        IAnimationContainer SpecialCursor { get; set; }
    }
}

