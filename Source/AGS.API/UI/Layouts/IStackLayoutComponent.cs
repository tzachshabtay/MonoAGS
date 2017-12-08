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
    [RequiredComponent(typeof(IBoundingBoxWithChildrenComponent))]
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

        /// <summary>
        /// Gets or sets the start location (the first item in the layout will be placed in that location).
        /// </summary>
        /// <value>The start location.</value>
        float StartLocation { get; set; }

        /// <summary>
        /// Starts applying the layout to its children (this needs to be called at least once).
        /// </summary>
        void StartLayout();

        /// <summary>
        /// Stops applying the layout to its children.
        /// </summary>
        void StopLayout();

        /// <summary>
        /// An event which fires whenever the layout changes.
        /// </summary>
        /// <value>The on layout changed event.</value>
        IBlockingEvent OnLayoutChanged { get; }

        /// <summary>
        /// Gets a list of entitiy IDs to ignore from the layout.
        /// </summary>
        /// <value>The entities to ignore.</value>
        IConcurrentHashSet<string> EntitiesToIgnore { get; }
    }
}
