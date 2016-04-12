using System;
using AGS.API;
using System.Drawing;

namespace AGS.Engine
{
	public class AGSBrush : IBrush
	{
		public AGSBrush(Brush brush)
		{
			InnerBrush = brush;
		}

		public Brush InnerBrush { get; private set; }

		public static AGSBrush Solid(IColor color)
		{
			AGSBrush brush = new AGSBrush (new SolidBrush (System.Drawing.Color.FromArgb(color.A, color.R, color.G, color.B)));
			brush.Type = BrushType.Solid;
			brush.Color = color;
			return brush;
		}

		#region IBrush implementation

		public BrushType Type { get; private set; }

		public IColor Color { get; private set; }

		public IBlend Blend { get; private set; }

		public bool GammaCorrection { get; private set; }

		public IColorBlend InterpolationColors { get; private set; }

		public IColor[] LinearColors { get; private set; }

		public ITransformMatrix Transform { get; private set; }

		public WrapMode WrapMode { get; private set; }

		public IColor BackgroundColor { get; private set; }

		public HatchStyle HatchStyle { get; private set; }

		public IPoint CenterPoint { get; private set; }

		public IPoint FocusScales { get; private set; }

		#endregion
	}
}

