using System;

namespace AGS.API
{
	public interface IBrushLoader
	{
		IBrush LoadSolidBrush(Color color);

		IBrush LoadLinearBrush(Point point1, Point point2, Color color1, Color color2);
		IBrush LoadLinearBrush(Color[] linearColors, IBlend blend, IColorBlend interpolationColors, 
			ITransformMatrix transform, WrapMode wrapMode, bool gammaCorrection);

		IBrush LoadHatchBrush(HatchStyle hatchStyle, Color color, Color backgroundColor);

		IBrush LoadPathsGradientBrush(Point[] points);
		IBrush LoadPathsGradientBrush(Color centerColor, PointF centerPoint, IBlend blend, PointF focusScales,
			Color[] surroundColors, IColorBlend interpolationColors, ITransformMatrix transform, WrapMode wrapMode);
	}
}

