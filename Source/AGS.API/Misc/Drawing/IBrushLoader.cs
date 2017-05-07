namespace AGS.API
{
    /// <summary>
    /// Loads a brush
    /// </summary>
	public interface IBrushLoader
	{
        /// <summary>
        /// Loads a solid color brush.
        /// </summary>
        /// <returns>The solid brush.</returns>
        /// <param name="color">Color.</param>
		IBrush LoadSolidBrush(Color color);

        /// <summary>
        /// Loads a linear color brush (this is currently only supported on desktop).
        /// </summary>
        /// <returns>The linear brush.</returns>
        /// <param name="point1">Point1.</param>
        /// <param name="point2">Point2.</param>
        /// <param name="color1">Color1.</param>
        /// <param name="color2">Color2.</param>
		IBrush LoadLinearBrush(Point point1, Point point2, Color color1, Color color2);

        /// <summary>
        /// Loads a linear color brush (this is currently only supported on desktop).
        /// </summary>
        /// <returns>The linear brush.</returns>
        /// <param name="linearColors">Linear colors.</param>
        /// <param name="blend">Blend.</param>
        /// <param name="interpolationColors">Interpolation colors.</param>
        /// <param name="transform">Transform.</param>
        /// <param name="wrapMode">Wrap mode.</param>
        /// <param name="gammaCorrection">If set to <c>true</c> gamma correction.</param>
		IBrush LoadLinearBrush(Color[] linearColors, IBlend blend, IColorBlend interpolationColors, 
			ITransformMatrix transform, WrapMode wrapMode, bool gammaCorrection);

		IBrush LoadHatchBrush(HatchStyle hatchStyle, Color color, Color backgroundColor);

		IBrush LoadPathsGradientBrush(Point[] points);
		IBrush LoadPathsGradientBrush(Color centerColor, PointF centerPoint, IBlend blend, PointF focusScales,
			Color[] surroundColors, IColorBlend interpolationColors, ITransformMatrix transform, WrapMode wrapMode);
	}
}

