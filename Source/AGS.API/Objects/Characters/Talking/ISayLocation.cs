namespace AGS.API
{
    /// <summary>
    /// Represents a location of where to render said text, and where to render a portrait (if a portrait should be rendered).
    /// </summary>
    public interface ISayLocation
    {
        /// <summary>
        /// Gets the location of where the text is to be rendered.
        /// </summary>
        /// <value>The text location.</value>
        PointF TextLocation { get; }

        /// <summary>
        /// Gets the location of where the portrait is to be rendered (or null for no portrait rendering).
        /// </summary>
        /// <value>The portrait location.</value>
        PointF? PortraitLocation { get; }
    }
}

