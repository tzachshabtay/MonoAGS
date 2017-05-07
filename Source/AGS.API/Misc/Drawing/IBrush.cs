namespace AGS.API
{
    /// <summary>
    /// A brush which can be used to draw. 
    /// </summary>
	public interface IBrush
	{
        /// <summary>
        /// Gets the brush type (currently only Solid is supported on all platforms, though other brushes are
        /// expected to work on desktop).
        /// </summary>
        /// <value>The type.</value>
		BrushType Type { get; }

        /// <summary>
        /// Gets the brush primary color.
        /// </summary>
        /// <value>The color.</value>
		Color Color { get; }

		IBlend Blend { get; }
		bool GammaCorrection { get; }
		IColorBlend InterpolationColors { get; }
		Color[] LinearColors { get; }
		ITransformMatrix Transform { get; }
		WrapMode WrapMode { get; }
		Color BackgroundColor { get; }
		HatchStyle HatchStyle { get; }
		PointF CenterPoint { get; }
		PointF FocusScales { get; }
	}
}

