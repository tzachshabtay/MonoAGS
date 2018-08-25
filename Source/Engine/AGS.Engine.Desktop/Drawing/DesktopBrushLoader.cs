using System;
using AGS.API;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace AGS.Engine.Desktop
{
	public class DesktopBrushLoader : IBrushLoader
	{
		#region IBrushLoader implementation

        [MethodWizard]
		public IBrush LoadSolidBrush(AGS.API.Color color)
		{
			return DesktopBrush.Solid(color);
		}

        [MethodWizard]
		public IBrush LoadLinearBrush(AGS.API.Point point1, AGS.API.Point point2, AGS.API.Color color1, AGS.API.Color color2)
		{
			return new DesktopBrush (new LinearGradientBrush (point1.Convert(), point2.Convert(), color1.Convert(), color2.Convert()));
		}

		public IBrush LoadLinearBrush(AGS.API.Color[] linearColors, IBlend blend, IColorBlend interpolationColors, 
			ITransformMatrix transform, AGS.API.WrapMode wrapMode, bool gammaCorrection)
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

		public IBrush LoadHatchBrush(AGS.API.HatchStyle hatchStyle, AGS.API.Color color, AGS.API.Color backgroundColor)
		{
			return new DesktopBrush(new HatchBrush(hatchStyle.Convert(), color.Convert(), backgroundColor.Convert()));
		}

		public IBrush LoadPathsGradientBrush(AGS.API.Point[] points)
		{
			return new DesktopBrush (new PathGradientBrush (points.Convert()));
		}

		public IBrush LoadPathsGradientBrush(AGS.API.Color centerColor, AGS.API.PointF centerPoint, 
					IBlend blend, AGS.API.PointF focusScales, AGS.API.Color[] surroundColors, 
					IColorBlend interpolationColors, ITransformMatrix transform, AGS.API.WrapMode wrapMode)
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

