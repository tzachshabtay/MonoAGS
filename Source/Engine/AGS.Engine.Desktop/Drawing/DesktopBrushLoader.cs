using System.Drawing.Drawing2D;
using AGS.API;
using HatchStyle = AGS.API.HatchStyle;
using WrapMode = AGS.API.WrapMode;

namespace AGS.Engine.Desktop
{
	public class DesktopBrushLoader : IBrushLoader
	{
		#region IBrushLoader implementation

		public IBrush LoadSolidBrush(Color color)
		{
			return DesktopBrush.Solid(color);
		}

		public IBrush LoadLinearBrush(Point point1, Point point2, Color color1, Color color2)
		{
			return new DesktopBrush (new LinearGradientBrush (point1.Convert(), point2.Convert(), color1.Convert(), color2.Convert()));
		}

		public IBrush LoadLinearBrush(Color[] linearColors, IBlend blend, IColorBlend interpolationColors, 
			ITransformMatrix transform, WrapMode wrapMode, bool gammaCorrection)
		{
			LinearGradientBrush g = new LinearGradientBrush (new System.Drawing.Point (), new System.Drawing.Point (), 
				System.Drawing.Color.White, System.Drawing.Color.White);
			g.Blend = blend.Convert();
			g.GammaCorrection = gammaCorrection;
			g.InterpolationColors = interpolationColors.Convert();
			g.LinearColors = linearColors.Convert();
			g.Transform = transform.Convert();
			g.WrapMode = wrapMode.Convert();
			return new DesktopBrush(g);
		}

		public IBrush LoadHatchBrush(HatchStyle hatchStyle, Color color, Color backgroundColor)
		{
			return new DesktopBrush(new HatchBrush(hatchStyle.Convert(), color.Convert(), backgroundColor.Convert()));
		}

		public IBrush LoadPathsGradientBrush(Point[] points)
		{
			return new DesktopBrush (new PathGradientBrush (points.Convert()));
		}

		public IBrush LoadPathsGradientBrush(Color centerColor, PointF centerPoint, 
					IBlend blend, PointF focusScales, Color[] surroundColors, 
					IColorBlend interpolationColors, ITransformMatrix transform, WrapMode wrapMode)
		{
			PathGradientBrush g = new PathGradientBrush (new System.Drawing.Point[]{ });
			g.Blend = blend.Convert();
			g.CenterColor = centerColor.Convert();
			g.CenterPoint = centerPoint.Convert();
			g.FocusScales = focusScales.Convert();
			g.SurroundColors = surroundColors.Convert();
			g.InterpolationColors = interpolationColors.Convert();
			g.Transform = transform.Convert();
			g.WrapMode = wrapMode.Convert();
			return new DesktopBrush(g);
		}

		#endregion
	}
}

