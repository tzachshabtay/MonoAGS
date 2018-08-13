using System;
namespace AGS.API
{
    /// <summary>
    /// A factory for creating borders (<see cref="IBorderStyle"/>).
    /// </summary>
    public interface IBorderFactory
    {
        /// <summary>
        /// Creates a solid color border.
        /// </summary>
        /// <returns>The border.</returns>
        /// <param name="color">Color.</param>
        /// <param name="lineWidth">Line width.</param>
        /// <param name="hasRoundCorners">If set to <c>true</c> has round corners.</param>
        IBorderStyle SolidColor(Color? color = null, float lineWidth = 10f, bool hasRoundCorners = false);

        /// <summary>
        /// Creates a gradient border (you can give a different color for each of the corners, and the border color will interpolate between them).
        /// </summary>
        /// <returns>The border.</returns>
        /// <param name="color">Color.</param>
        /// <param name="lineWidth">Line width.</param>
        /// <param name="hasRoundCorners">If set to <c>true</c> has round corners.</param>
        IBorderStyle Gradient(FourCorners<Color> color, float lineWidth = 10f, bool hasRoundCorners = false);

        /// <summary>
        /// Combines multiple borders into a single border.
        /// </summary>
        /// <returns>The border.</returns>
        /// <param name="borders">Borders.</param>
        IBorderStyle Multiple(params IBorderStyle[] borders);
    }
}