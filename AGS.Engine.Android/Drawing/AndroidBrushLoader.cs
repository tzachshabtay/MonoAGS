using System;
using AGS.API;
using Android.Graphics;

namespace AGS.Engine.Android
{
	public class AndroidBrushLoader : IBrushLoader
	{
		public AndroidBrushLoader()
		{
		}

		#region IBrushLoader implementation

		public IBrush LoadSolidBrush(AGS.API.Color color)
		{
			return AndroidBrush.Solid(color);
		}

		public IBrush LoadLinearBrush(AGS.API.Point point1, AGS.API.Point point2, AGS.API.Color color1, AGS.API.Color color2)
		{
			var paint = AndroidBrush.CreateTextPaint();
			LinearGradient gradient = new LinearGradient (point1.X, point1.Y, point2.X, point2.Y, color1.Convert(), color2.Convert(), Shader.TileMode.Clamp);
			paint.SetShader(gradient);
			return new AndroidBrush (paint);
		}

		public IBrush LoadLinearBrush(AGS.API.Color[] linearColors, IBlend blend, IColorBlend interpolationColors, ITransformMatrix transform, WrapMode wrapMode, bool gammaCorrection)
		{
			throw new NotImplementedException();
		}

		public IBrush LoadHatchBrush(HatchStyle hatchStyle, AGS.API.Color color, AGS.API.Color backgroundColor)
		{
			throw new NotImplementedException();
		}

		public IBrush LoadPathsGradientBrush(AGS.API.Point[] points)
		{
			throw new NotImplementedException();
		}

		public IBrush LoadPathsGradientBrush(AGS.API.Color centerColor, AGS.API.PointF centerPoint, IBlend blend, AGS.API.PointF focusScales, AGS.API.Color[] surroundColors, IColorBlend interpolationColors, ITransformMatrix transform, WrapMode wrapMode)
		{
			throw new NotImplementedException();
		}

		#endregion
	}
}

