using System;

namespace AGS.API
{
	public interface IBrush
	{
		BrushType Type { get; }
		IColor Color { get; }
		IBlend Blend { get; }
		bool GammaCorrection { get; }
		IColorBlend InterpolationColors { get; }
		IColor[] LinearColors { get; }
		ITransformMatrix Transform { get; }
		WrapMode WrapMode { get; }
		IColor BackgroundColor { get; }
		HatchStyle HatchStyle { get; }
		IPoint CenterPoint { get; }
		IPoint FocusScales { get; }
	}
}

