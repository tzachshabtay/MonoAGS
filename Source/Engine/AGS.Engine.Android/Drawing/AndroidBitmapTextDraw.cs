using System;
using AGS.API;
using Android.Graphics;
using Android.Text;

namespace AGS.Engine.Android
{
	public class AndroidBitmapTextDraw : IBitmapTextDraw
	{
		private ITextConfig _config;
		private Bitmap _bitmap;
		private int _maxWidth;//, _height;
		private string _text;
        private Canvas _canvas;

		public AndroidBitmapTextDraw(Bitmap bitmap)
		{
			_bitmap = bitmap;
		}

        #region IBitmapTextDraw implementation

        public IDisposable CreateContext()
        {
            _canvas = new Canvas(_bitmap);
            _canvas.DrawColor(global::Android.Graphics.Color.Transparent, PorterDuff.Mode.Clear);
            return _canvas;
        }

		public void DrawText(string text, ITextConfig config, AGS.API.SizeF textSize, AGS.API.SizeF baseSize, int maxWidth, int height, float xOffset)
		{
			//_height = height; todo: support height
			_text = text;
			_config = config;
			_maxWidth = maxWidth;

			TextPaint paint = getPaint(_config.Brush);

			float left = xOffset + _config.AlignX(textSize.Width, baseSize);
			float top = _config.AlignY(_bitmap.Height, textSize.Height, baseSize);
			float centerX = left + _config.OutlineWidth / 2f;
			float centerY = top + _config.OutlineWidth / 2f;
			float right = left + _config.OutlineWidth;
			float bottom = top + _config.OutlineWidth;

            var canvas = _canvas;
			if (_config.OutlineWidth > 0f)
			{
				TextPaint outlinePaint = getPaint(_config.OutlineBrush);
				drawString(canvas, outlinePaint, left, top);
				drawString(canvas, outlinePaint, centerX, top);
				drawString(canvas, outlinePaint, right, top);

				drawString(canvas, outlinePaint, left, centerY);
				drawString(canvas, outlinePaint, right, centerY);

				drawString(canvas, outlinePaint, left, bottom);
				drawString(canvas, outlinePaint, centerX, bottom);
				drawString(canvas, outlinePaint, right, bottom);
			}
			if (_config.ShadowBrush != null)
			{
				TextPaint shadowPaint = getPaint(_config.ShadowBrush);
				drawString(canvas, shadowPaint, centerX + _config.ShadowOffsetX, 
					centerY + _config.ShadowOffsetY);
			}
			drawString(canvas, paint, centerX, centerY);
		}

		#endregion

		private TextPaint getPaint(IBrush brush)
		{
			TextPaint paint = ((AndroidBrush)brush).InnerBrush;
			paint.SetTypeface(((AndroidFont)_config.Font).InnerFont);
			paint.TextSize = _config.Font.SizeInPoints;
			return paint;
		}

		private void drawString(Canvas gfx, TextPaint paint, float x, float y)
		{
			if (_maxWidth == int.MaxValue)
			{
				gfx.DrawText(_text, x, y, paint);
			}
			else
			{
				paint.TextAlign = alignWrap();
				StaticLayout layout = new StaticLayout (_text, paint, _maxWidth, Layout.Alignment.AlignNormal, 1f, 0f, false);
				gfx.Translate(x, y);
				layout.Draw(gfx);
				gfx.Translate(-x, -y);
			}
		}

		private Paint.Align alignWrap()
		{
			switch (_config.Alignment)
			{
				case Alignment.TopLeft:
				case Alignment.MiddleLeft:
				case Alignment.BottomLeft:
					return Paint.Align.Left;
				case Alignment.TopCenter:
				case Alignment.MiddleCenter:
				case Alignment.BottomCenter:
					return Paint.Align.Center;
				default:
					return Paint.Align.Right;
			}
		}
	}
}

