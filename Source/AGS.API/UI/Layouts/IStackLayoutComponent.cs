namespace AGS.API
{
	/// <summary>
	/// The stack layout component organizes its children in a one-dimensional line ("stack"), 
    /// either horizontally, vertically or diagonally.
    /// The default is a simple vertical layout (top to bottom).
	/// </summary>
	[RequiredComponent(typeof(IInObjectTree))]
    public interface IStackLayoutComponent : IComponent
    {
        /// <summary>
        /// Gets or sets the absolute spacing between each item in the layout.
        /// This is an extra constant spacing that will be applied on top of the <see cref="RelativeSpacing"/>.
        /// Set the X spacing for an extra horizontal spacing, set the Y spacing for an extra vertical spacing,
        /// or set them both for an extra diagonal spacing.
        /// Set a positive x for left-to-right or negative x for right-to-left.
        /// Set a positive y for bottom-to-top or negative y for top-to-bottom.
        /// </summary>
        /// <value>The spacing.</value>
        PointF AbsoluteSpacing { get; set; }

        /// <summary>
        /// Gets or sets the relative spacing between each item in the layout.
        /// This is a factor which will multiply by the size of each item. 
        /// Set the X spacing for a width multiplier (1 will assign left-to-right, -1 will assign right-to-left, 0 means no horizontal layout).
        /// Set the Y spacing for a height multiplier (1 will assign bottom-to-top, -1 will assign top-to-bottom, 0 means no vertical layout).
        /// </summary>
        /// <value>The relative spacing.</value>
        PointF RelativeSpacing { get; set; }
    }
}
