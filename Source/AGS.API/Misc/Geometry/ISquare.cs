namespace AGS.API
{
    /// <summary>
    /// Represents a square.
    /// 
    /// Note: the points are listed as bottom left, bottom right, etc, even though it's not accurate.
    /// A square doesn't really have bottom left, it just has 4 points. 
    /// We use these descriptors for convenience, since each square in our engine was converted from
    /// a rectangle which does have bottom left, etc.
    /// Also, the order of the points is important for the "Contains" calculation.
    /// </summary>
    public interface ISquare
	{
        /// <summary>
        /// Gets the bottom left point.
        /// </summary>
        /// <value>The bottom left.</value>
		PointF BottomLeft { get; }

        /// <summary>
        /// Gets the bottom right point.
        /// </summary>
        /// <value>The bottom right.</value>
		PointF BottomRight { get; }

        /// <summary>
        /// Gets the top left point.
        /// </summary>
        /// <value>The top left.</value>
		PointF TopLeft { get; }

        /// <summary>
        /// Gets the top right point.
        /// </summary>
        /// <value>The top right.</value>
		PointF TopRight { get; }

        /// <summary>
        /// Gets the minimum x of the square.
        /// </summary>
        /// <value>The minimum x.</value>
		float MinX { get; }

        /// <summary>
        /// Gets the maximum x of the square.
        /// </summary>
        /// <value>The max x.</value>
		float MaxX { get; }

        /// <summary>
        /// Gets the minimum y of the square.
        /// </summary>
        /// <value>The minimum y.</value>
		float MinY { get; }

        /// <summary>
        /// Gets the maximum y of the square.
        /// </summary>
        /// <value>The max y.</value>
		float MaxY { get; }

        /// <summary>
        /// Is the specified point contained in the square?
        /// </summary>
        /// <returns>True if the point is in the square, false otherwise.</returns>
        /// <param name="point">Point.</param>
		bool Contains(PointF point);

        /// <summary>
        /// Create a new square which is flipped horizontally from the current square.
        /// </summary>
        /// <returns>The new flipped square.</returns>
		ISquare FlipHorizontal();
	}
}

