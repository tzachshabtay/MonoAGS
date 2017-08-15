namespace AGS.API
{
    /// <summary>
    /// A border style will draw a border around an entity.
    /// It can draw both behind and in front of the entity.
    /// 
    /// There are several borders that come built in:
    /// 1. A solid color border (use AGSBorders.SolidColor): allows to set a color, line width and optional rounded corners.
    /// 2. A gradient color border (use AGSBorders.Gradient): allows to set 4 colors for the 4 corners which will be interpolated across, a line width and optional rounded corners.
    /// 3. A 9-slice image border (use AGSSlicedImageBorder): this allows you to use an image, slice it to 9 pieces, and spread it in various ways to create a border.
    /// The 9-slice image border is heavily inspired by the border image used by CSS, so you can look at an example in CSS, to
    /// see what can be done with a 9-slice image: https://css-tricks.com/almanac/properties/b/border-image/
    /// 
    /// You can also implement your own custom border style by implementing this interface, which can then be used
    /// for all objects.
    /// </summary>
    public interface IBorderStyle
	{
        /// <summary>
        /// Renders border graphics which will appear behind the entity.
        /// This is called on every game tick.
        /// </summary>
        /// <param name="square">Square.</param>
		void RenderBorderBack(AGSBoundingBox square);

        /// <summary>
        /// Renders border graphics which will appear in front of the entity.
        /// This is called on every game tick.
        /// </summary>
        /// <param name="square">Square.</param>
		void RenderBorderFront(AGSBoundingBox square);

        /// <summary>
        /// Gets the left edge width of the border.
        /// </summary>
        /// <value>The left width.</value>
        float WidthLeft { get; }

        /// <summary>
        /// Gets the right edge width of the border.
        /// </summary>
        /// <value>The right width.</value>
        float WidthRight { get; }

        /// <summary>
        /// Gets the top edge width of the border.
        /// </summary>
        /// <value>The top width.</value>
        float WidthTop { get; }

        /// <summary>
        /// Gets the bottom edge width of the border.
        /// </summary>
        /// <value>The bottom width.</value>
        float WidthBottom { get; }
	}
}

