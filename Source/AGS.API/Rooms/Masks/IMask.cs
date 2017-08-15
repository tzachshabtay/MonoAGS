namespace AGS.API
{
    /// <summary>
    /// A mask is basically a 2D array of booleans, usually representing an area on the screen.
    /// </summary>
    public interface IMask
	{
        /// <summary>
        /// Gets the width.
        /// </summary>
        /// <value>The width.</value>
		int Width { get; }

        /// <summary>
        /// Gets the height.
        /// </summary>
        /// <value>The height.</value>
		int Height { get; }

        /// <summary>
        /// When loading the mask there's an option for drawing the mask on the screen, for debugging purposes.
        /// </summary>
        /// <value>The debug draw.</value>
		IObject DebugDraw { get; }

        /// <summary>
        /// The minimum X with a "masked" (i.e a true) value.
        /// </summary>
        /// <value>The minimum x.</value>
		float MinX { get; }

        /// <summary>
        /// The maximum X with a "masked" (i.e a true) value.
        /// </summary>
        /// <value>The max x.</value>
		float MaxX { get; }

        /// <summary>
        /// The minimum Y with a "masked" (i.e a true) value.
        /// </summary>
        /// <value>The minimum y.</value>
		float MinY { get; }

        /// <summary>
        /// The maximum Y with a "masked" (i.e a true) value.
        /// </summary>
        /// <value>The max y.</value>
		float MaxY { get; }

        /// <summary>
        /// Is the specific point masked (i.e has a true value in the 2D array)?
        /// </summary>
        /// <returns><c>true</c>, if masked, <c>false</c> otherwise.</returns>
        /// <param name="point">Point.</param>
		bool IsMasked(PointF point);

        /// <summary>
        /// Is the specific point masked after projecting the mask on the specified square?
        /// </summary>
        /// <returns><c>true</c>, if masked, <c>false</c> otherwise.</returns>
        /// <param name="point">Point.</param>
        /// <param name="projectionBox">Projection box.</param>
        /// <param name="scaleX">The projection box x scaling (is only needed to know if the projection box is flipped horizontally).</param>
        /// <param name="scaleY">The projection box y scaling (is only need to know if the projection box is flipped vertically).</param>
		bool IsMasked(PointF point, AGSBoundingBox projectionBox, float scaleX, float scaleY);

        /// <summary>
        /// Returns the mask as a jagged array (https://msdn.microsoft.com/en-us/library/2s05feca.aspx).
        /// </summary>
        /// <returns>The jagged array.</returns>
		bool[][] AsJaggedArray();

        /// <summary>
        /// Copies the mask to a 2D array and returns it.
        /// </summary>
        /// <returns>The DA rray.</returns>
		bool[,] To2DArray();

        /// <summary>
        /// Adds the current mask to the target specified mask (i.e all true values in the current mask will be set to true in the target mask).
        /// </summary>
        /// <param name="mask">Mask.</param>
		void ApplyToMask(bool[][] mask);

        /// <summary>
        /// Returns a display string with the mask contents as asterisks, for debugging purposes.
        /// </summary>
        /// <returns>The string.</returns>
		string DebugString();
	}
}

