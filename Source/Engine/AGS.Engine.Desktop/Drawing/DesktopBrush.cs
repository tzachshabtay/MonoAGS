using System;
using AGS.API;
using System.Drawing;

namespace AGS.Engine.Desktop
{
    [PropertyFolder]
	public class DesktopBrush : IBrush
	{
		public DesktopBrush(Brush brush)
		{
			InnerBrush = brush;
		}

		public Brush InnerBrush { get; private set; }

		public static DesktopBrush Solid(API.Color color)
		{
			DesktopBrush brush = new DesktopBrush (new SolidBrush (color.Convert()));
			brush.Type = BrushType.Solid;
			brush.Color = color;
			return brush;
		}

		public static DesktopBrush Solid(System.Drawing.Color color)
		{
			DesktopBrush brush = new DesktopBrush (new SolidBrush (color));
			brush.Type = BrushType.Solid;
			brush.Color = color.Convert();
			return brush;
		}

		#region IBrush implementation

		public BrushType Type { get; private set; }

		public API.Color Color { get; private set; }

		public IBlend Blend { get; private set; }

		public bool GammaCorrection { get; private set; }

		public IColorBlend InterpolationColors { get; private set; }

		public AGS.API.Color[] LinearColors { get; private set; }

		public ITransformMatrix Transform { get; private set; }

		public WrapMode WrapMode { get; private set; }

		public AGS.API.Color BackgroundColor { get; private set; }

		public HatchStyle HatchStyle { get; private set; }

		public AGS.API.PointF CenterPoint { get; private set; }

		public AGS.API.PointF FocusScales { get; private set; }

		#endregion
	}
}

