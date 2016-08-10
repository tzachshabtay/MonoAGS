using System;

namespace AGS.API
{
	public interface IBrush
	{
		BrushType Type { get; }
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

