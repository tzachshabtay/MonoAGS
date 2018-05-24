using System;
using System.Linq;
using System.Drawing;
using System.Drawing.Text;
using System.Drawing.Drawing2D;
using System.Threading;
using System.Collections.Concurrent;

namespace AGS.Engine.Desktop
{
	public static class Extensions
	{
        private static readonly ThreadLocal<Graphics> _graphics;

        private struct TextMeasureKey
        {
            public TextMeasureKey(string text, Font font, int maxWidth)
            {
                Text = text;
                Font = font;
                MaxWidth = maxWidth;
            }
            public string Text;
            public Font Font;
            public int MaxWidth;
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

        public static System.Drawing.SizeF Measure(this string text, Font font, int maxWidth = int.MaxValue)
		{
            try
            {
                var key = new TextMeasureKey(text, font, maxWidth);
                var size = _measurements.GetOrAdd(key,
                          k =>
                {
                    lock (DesktopBitmapTextDraw.GraphicsLocker)
                    {
                        return _graphics.Value.MeasureString(k.Text, k.Font, k.MaxWidth, StringFormat.GenericTypographic);
                    }
                });
                return size;
            }
            catch (ObjectDisposedException)
            {
                return SizeF.Empty;
            }
		}

		public static System.Drawing.Color Convert(this AGS.API.Color color)
		{
			return Color.FromArgb((int)color.Value);
		}

		public static System.Drawing.Color[] Convert(this AGS.API.Color[] colors)
		{
			return colors.Select(Convert).ToArray();
		}

		public static AGS.API.Color Convert(this System.Drawing.Color color)
		{
			return AGS.API.Color.FromHexa((uint)color.ToArgb());
		}

		public static System.Drawing.PointF Convert(this AGS.API.PointF point)
		{
			return new System.Drawing.PointF (point.X, point.Y);
		}

		public static System.Drawing.Point Convert(this AGS.API.Point point)
		{
			return new System.Drawing.Point (point.X, point.Y);
		}

		public static System.Drawing.Point[] Convert(this AGS.API.Point[] points)
		{
			return points.Select(Convert).ToArray();
		}

		public static System.Drawing.Drawing2D.WrapMode Convert(this AGS.API.WrapMode wrapMode)
		{
			return (System.Drawing.Drawing2D.WrapMode)wrapMode;
		}

		public static System.Drawing.Drawing2D.Blend Convert(this AGS.API.IBlend blend)
		{
			var result = new System.Drawing.Drawing2D.Blend(blend.Positions.Length);
			result.Positions = blend.Positions;
			result.Factors = blend.Factors;
			return result;
		}

		public static System.Drawing.Drawing2D.ColorBlend Convert(this AGS.API.IColorBlend blend)
		{
			var result = new System.Drawing.Drawing2D.ColorBlend (blend.Colors.Length);
			result.Colors = blend.Colors.Convert();
			result.Positions = blend.Positions;
			return result;
		}

		public static System.Drawing.Drawing2D.Matrix Convert(this AGS.API.ITransformMatrix transform)
		{
			var result = new System.Drawing.Drawing2D.Matrix (transform.Elements[0],
				transform.Elements[1], transform.Elements[2], transform.Elements[3],
				transform.Elements[4], transform.Elements[5]);
			return result;
		}

		public static System.Drawing.Drawing2D.HatchStyle Convert(this AGS.API.HatchStyle hatchStyle)
		{
			return (System.Drawing.Drawing2D.HatchStyle)hatchStyle;
		}

		public static System.Drawing.FontStyle Convert(this AGS.API.FontStyle fontStyle)
		{
			return (System.Drawing.FontStyle)fontStyle;
		}

		public static System.Drawing.Rectangle Convert (this AGS.API.Rectangle rectangle)
		{
			return new Rectangle (rectangle.X, rectangle.Y, rectangle.Width, rectangle.Height);
		}
	}
}

