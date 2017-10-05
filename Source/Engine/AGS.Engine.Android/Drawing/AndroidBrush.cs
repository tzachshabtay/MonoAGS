using System;
using AGS.API;
using Android.Text;

namespace AGS.Engine.Android
{
    [PropertyFolder]
	public class AndroidBrush : IBrush
	{
		public AndroidBrush(TextPaint paint)
		{
			InnerBrush = paint;
		}

		public static TextPaint CreateTextPaint()
		{
			return new TextPaint (global::Android.Graphics.PaintFlags.AntiAlias | 
				global::Android.Graphics.PaintFlags.SubpixelText);
		}

		public static AndroidBrush Solid(AGS.API.Color color)
		{
			TextPaint paint = CreateTextPaint();
			paint.Color = color.Convert();
			AndroidBrush brush = new AndroidBrush (paint);
			brush.Type = BrushType.Solid;
			brush.Color = color;
			return brush;
		}

		public TextPaint InnerBrush { get; private set; }

		#region IBrush implementation

		public BrushType Type { get; private set; }

		public Color Color { get; private set; }

		public IBlend Blend { get; private set; }

		public bool GammaCorrection { get; private set; }

		public IColorBlend InterpolationColors { get; private set; }

		public Color[] LinearColors { get; private set; }

		public ITransformMatrix Transform { get; private set; }

		public WrapMode WrapMode { get; private set; }

		public Color BackgroundColor { get; private set; }

		public HatchStyle HatchStyle { get; private set; }

		public PointF CenterPoint { get; private set; }

		public PointF FocusScales { get; private set; }

		#endregion
	}
}

