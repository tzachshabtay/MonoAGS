using System;
using AGS.API;
using SkiaSharp;

namespace AGS.Engine
{
    public class SkiaTextDraw : IBitmapTextDraw
    {
        private ITextConfig _config;
        private SKBitmap _bitmap;
        private SKCanvas _canvas;
        private int _maxWidth, _height;
        private string _text;

        public static readonly object GraphicsLocker = new object();

        public SkiaTextDraw(SKBitmap bitmap)
        {
            _bitmap = bitmap;
        }

        public IDisposable CreateContext()
        {
            _canvas = new SKCanvas(_bitmap);
            _canvas.Clear(SKColors.Transparent);
            return _canvas;
        }

        public void DrawText(string text, ITextConfig config, API.SizeF textSize, API.SizeF baseSize,
            int maxWidth, int height, float xOffset)
        {
            _height = height;
            _text = text;
            _config = config;
            _maxWidth = maxWidth;
            IFont font = _config.Font;
            IBrush outlineBrush = _config.OutlineBrush;

            float left = xOffset + _config.AlignX(textSize.Width, baseSize);
            float top = _config.AlignY(_bitmap.Height, textSize.Height, baseSize);
            float centerX = left + _config.OutlineWidth / 2f;
            float centerY = top + _config.OutlineWidth / 2f;
            float right = left + _config.OutlineWidth;
            float bottom = top + _config.OutlineWidth;

            lock (GraphicsLocker)
            {
                if (_config.OutlineWidth > 0f)
                {
                    drawString(outlineBrush, left, top);
                    drawString(outlineBrush, centerX, top);
                    drawString(outlineBrush, right, top);

                    drawString(outlineBrush, left, centerY);
                    drawString(outlineBrush, right, centerY);

                    drawString(outlineBrush, left, bottom);
                    drawString(outlineBrush, centerX, bottom);
                    drawString(outlineBrush, right, bottom);
                }
                if (_config.ShadowBrush != null)
                {
                    drawString(_config.ShadowBrush, centerX + _config.ShadowOffsetX,
                        centerY + _config.ShadowOffsetY);
                }
                drawString(_config.Brush, centerX, centerY);
            }

            //This should be a better way to render the outline (DrawPath renders the outline, and FillPath renders the text)
            //but for some reason some lines are missing when we render like that, at least on the mac
            /*if (_outlineWidth > 0f)
            {
                GraphicsPath path = new GraphicsPath ();
                Pen outlinePen = new Pen (_outlineBrush, _outlineWidth) { LineJoin = LineJoin.Round };
                path.AddString(_text, _font.FontFamily, (int)_font.Style, _font.Size, new Point (), new StringFormat ());
                //gfx.ScaleTransform(1.3f, 1.35f);
                gfx.DrawPath(outlinePen, path);
                gfx.FillPath(_brush, path);
            }
            else 
                gfx.DrawString (_text, _font, _brush, 0f, 0f);*/
        }

        private void drawString(IBrush brush, float x, float y)
        {
            var paint = getPaint(brush, _config.Font);
            if (_maxWidth == int.MaxValue)
            {
                _canvas.DrawText(_text, new SKPoint(x, y), paint);
            }
            else
            {
                //todo: draw in rectangle
                _canvas.DrawText(_text, new SKPoint(x, y), paint);
                //_canvas.DrawTextOnPath()
                //_canvas.DrawPositionedText(_text, )
                //gfx.DrawString(_text, font, brush, new System.Drawing.RectangleF(x, y, _maxWidth, _height),
                //    _wrapFormat);
            }
        }

        private SKPaint getPaint(IBrush brush, IFont font)
        {
            var paint = ((SkiaFont)font).Paint;
            paint.Color = brush.Color.Convert();
            return paint;
        }
    }
}