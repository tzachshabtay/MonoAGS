﻿using System;
using System.Collections.Concurrent;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.Linq;
using System.Threading;
using AGS.API;
using Color = System.Drawing.Color;
using FontStyle = System.Drawing.FontStyle;
using HatchStyle = System.Drawing.Drawing2D.HatchStyle;
using Point = System.Drawing.Point;
using PointF = System.Drawing.PointF;
using Rectangle = System.Drawing.Rectangle;
using SizeF = System.Drawing.SizeF;
using WrapMode = System.Drawing.Drawing2D.WrapMode;

namespace AGS.Engine.Desktop
{
	public static class Extensions
	{
        private static readonly ThreadLocal<Graphics> _graphics;

        private static StringFormat _leftFormat = new StringFormat(StringFormat.GenericTypographic) { Alignment = StringAlignment.Near };
        private static StringFormat _centerFormat = new StringFormat(StringFormat.GenericTypographic) { Alignment = StringAlignment.Center };
        private static StringFormat _rightFormat = new StringFormat(StringFormat.GenericTypographic) { Alignment = StringAlignment.Far };

        private struct TextMeasureKey
        {
            public TextMeasureKey(string text, Font font, int maxWidth, API.Alignment alignment)
            {
                Text = text;
                Font = font;
                MaxWidth = maxWidth;
                Alignment = alignment;
            }
            public string Text;
            public Font Font;
            public int MaxWidth;
            public API.Alignment Alignment;
        }

        private static readonly ConcurrentDictionary<TextMeasureKey, SizeF> _measurements = 
            new ConcurrentDictionary<TextMeasureKey, SizeF>();

        static Extensions()
        {
            _graphics = new ThreadLocal<Graphics>(() => 
            {
                var graphics = Graphics.FromImage(new Bitmap(1, 1));
                graphics.Init();
                return graphics;
            });
        }

        public static void Init(this Graphics gfx)
        {
            gfx.SmoothingMode = SmoothingMode.AntiAlias;
            gfx.InterpolationMode = InterpolationMode.HighQualityBicubic;
            gfx.TextRenderingHint = TextRenderingHint.AntiAlias;
            gfx.PixelOffsetMode = PixelOffsetMode.HighQuality;
            gfx.CompositingQuality = CompositingQuality.HighQuality;
        }

        public static void Clear(this Bitmap bitmap, Color color)
		{
			//todo: Possibly improve performance by using direct access math
			Graphics g = Graphics.FromImage(bitmap);
			g.Clear(color);
		}

		public static void Clear(this Bitmap bitmap)
		{
			bitmap.Clear(Color.White);
		}

        public static System.Drawing.SizeF Measure(this string text, Font font, API.Alignment alignment, int maxWidth = int.MaxValue)
		{
            try
            {
                var key = new TextMeasureKey(text, font, maxWidth, alignment);
                var size = _measurements.GetOrAdd(key,
                          k =>
                {
                    var format = alignment.GetFormat(maxWidth != int.MaxValue);
                    lock (DesktopBitmapTextDraw.GraphicsLocker)
                    {
                        return _graphics.Value.MeasureString(k.Text, k.Font, k.MaxWidth, format);
                    }
                });
                return size;
            }
            catch (ObjectDisposedException)
            {
                return SizeF.Empty;
            }
		}

        public static StringFormat GetFormat(this API.Alignment alignment, bool wrap)
        {
            if (!wrap) return StringFormat.GenericTypographic;
            switch (alignment)
            {
                case API.Alignment.BottomCenter:
                case API.Alignment.MiddleCenter:
                case API.Alignment.TopCenter:
                    return _centerFormat;
                case API.Alignment.BottomLeft:
                case API.Alignment.MiddleLeft:
                case API.Alignment.TopLeft:
                    return _leftFormat;
                case API.Alignment.BottomRight:
                case API.Alignment.MiddleRight:
                case API.Alignment.TopRight:
                    return _rightFormat;
                default:
                    throw new NotSupportedException(alignment.ToString());
            }
        }

		public static System.Drawing.Color Convert(this AGS.API.Color color)
		{
			return Color.FromArgb((int)color.Value);
		}

		public static Color[] Convert(this API.Color[] colors)
		{
			return colors.Select(Convert).ToArray();
		}

		public static API.Color Convert(this Color color)
		{
			return API.Color.FromHexa((uint)color.ToArgb());
		}

		public static PointF Convert(this API.PointF point)
		{
			return new PointF (point.X, point.Y);
		}

		public static Point Convert(this API.Point point)
		{
			return new Point (point.X, point.Y);
		}

		public static Point[] Convert(this API.Point[] points)
		{
			return points.Select(Convert).ToArray();
		}

		public static WrapMode Convert(this API.WrapMode wrapMode)
		{
			return (WrapMode)wrapMode;
		}

		public static Blend Convert(this IBlend blend)
		{
			var result = new Blend(blend.Positions.Length);
			result.Positions = blend.Positions;
			result.Factors = blend.Factors;
			return result;
		}

		public static ColorBlend Convert(this IColorBlend blend)
		{
			var result = new ColorBlend (blend.Colors.Length);
			result.Colors = blend.Colors.Convert();
			result.Positions = blend.Positions;
			return result;
		}

		public static Matrix Convert(this ITransformMatrix transform)
		{
			var result = new Matrix (transform.Elements[0],
				transform.Elements[1], transform.Elements[2], transform.Elements[3],
				transform.Elements[4], transform.Elements[5]);
			return result;
		}

		public static HatchStyle Convert(this API.HatchStyle hatchStyle)
		{
			return (HatchStyle)hatchStyle;
		}

		public static FontStyle Convert(this API.FontStyle fontStyle)
		{
			return (FontStyle)fontStyle;
		}

		public static Rectangle Convert (this API.Rectangle rectangle)
		{
			return new Rectangle (rectangle.X, rectangle.Y, rectangle.Width, rectangle.Height);
		}
	}
}

