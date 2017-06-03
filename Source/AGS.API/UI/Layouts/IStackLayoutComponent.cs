namespace AGS.API
{
    public enum LayoutDirection
    {
        Vertical,
        Horizontal
    }

	/// <summary>
	/// The stack layout component organizes its children in a one-dimensional line ("stack"), 
    /// either horizontally, or vertically.
    /// The default is a simple vertical layout (top to bottom).
	/// </summary>
	[RequiredComponent(typeof(IInObjectTree))]
    public interface IStackLayoutComponent : IComponent
    {
        /// <summary>
        /// Gets or sets the direction of the layout (horizontal or vertical).
        /// </summary>
        /// <value>The direction.</value>
        LayoutDirection Direction { get; set; }

        /// <summary>
        /// Gets or sets the absolute spacing between each item in the layout.
        /// This is an extra constant spacing that will be applied on top of the <see cref="RelativeSpacing"/>.
        /// This will either set the X or the Y of the children depending on the <see cref="Direction"/> of the layout. 
        /// Set a positive x for left-to-right or negative x for right-to-left.
        /// Set a positive y for bottom-to-top or negative y for top-to-bottom.
        /// </summary>
        /// <value>The spacing.</value>
        float AbsoluteSpacing { get; set; }

        /// <summary>
        /// Gets or sets the relative spacing between each item in the layout.
        /// This is a factor which will multiply by the size of each item. 
        /// This will either set the X or the Y of the children depending on the <see cref="Direction"/> of the layout. 
        /// Set the X spacing for a width multiplier (1 will assign left-to-right, -1 will assign right-to-left).
        /// Set the Y spacing for a height multiplier (1 will assign bottom-to-top, -1 will assign top-to-bottom).
        /// </summary>
        /// <value>The relative spacing.</value>
        float RelativeSpacing { get; set; }
    }
}
