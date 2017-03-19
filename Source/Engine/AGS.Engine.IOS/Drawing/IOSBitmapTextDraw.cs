extern alias IOS;

using System;
using AGS.API;
using IOS::CoreGraphics;
using IOS::CoreText;
using IOS::Foundation;
using IOS::UIKit;

namespace AGS.Engine.IOS
{
    public class IOSBitmapTextDraw : IBitmapTextDraw
    {
        private class IOSTextDrawContext : IDisposable
        {
            private Action<UIImage> _setImage;
            private CGContext _context;

            public IOSTextDrawContext(Action<UIImage> setImage)
            {
                _setImage = setImage;
            }

            public void Dispose()
            {
                var image = UIGraphics.GetImageFromCurrentImageContext();
                _setImage(image);
                UIGraphics.EndImageContext();
            }
        }

        private readonly UIImage _image;
        private readonly Action<UIImage> _setImage;
        private CGContext _context;
        private int _maxWidth, _height;
        private ITextConfig _config;
        private string _text;

        public IOSBitmapTextDraw(UIImage image, Action<UIImage> setImage)
        {
            _image = image;
            _setImage = setImage;
        }

        public IDisposable CreateContext()
        {
            UIGraphics.BeginImageContext(_image.Size);
            _context = UIGraphics.GetCurrentContext();
            _context.ClearRect(new CGRect(0, 0, _image.Size.Width, _image.Size.Height));
            return new IOSTextDrawContext(_setImage);
        }

        public void DrawText(string text, ITextConfig config, SizeF textSize, SizeF baseSize, int maxWidth, int height, float xOffset)
        {
            _maxWidth = maxWidth;
            _height = height;
            _config = config;
            _text = text;

            float left = xOffset + _config.AlignX(textSize.Width, baseSize);
            float top = _config.AlignY(_image.CGImage.Height, textSize.Height, baseSize);
            float centerX = left + _config.OutlineWidth / 2f;
            float centerY = top + _config.OutlineWidth / 2f;
            float right = left + _config.OutlineWidth;
            float bottom = top + _config.OutlineWidth;

            if (_config.OutlineWidth > 0f)
            {
                IBrush brush = _config.OutlineBrush;
                drawString(brush, left, top);
                drawString(brush, centerX, top);
                drawString(brush, right, top);
                           
                drawString(brush, left, centerY);
                drawString(brush, right, centerY);
                           
                drawString(brush, left, bottom);
                drawString(brush, centerX, bottom);
                drawString(brush, right, bottom);
            }
            if (_config.ShadowBrush != null)
            {
                drawString(_config.ShadowBrush, centerX + _config.ShadowOffsetX,
                    centerY + _config.ShadowOffsetY);
            }
            drawString(_config.Brush, centerX, centerY);
        }

        private void drawString(IBrush brush, float x, float y)
        {
            using (NSMutableAttributedString str = new NSMutableAttributedString(_text))
            {
                NSRange range = new NSRange(0, _text.Length);
                using (CTParagraphStyle style = new CTParagraphStyle(new CTParagraphStyleSettings { Alignment = align() }))
                {
                    var foreColor = brush.Color;
                    using (CGColor color = new CGColor(foreColor.R / 255f, foreColor.G / 255f, foreColor.B / 255f, foreColor.A / 255f))
                    {
                        str.SetAttributes(new CTStringAttributes
                        {
                            Font = ((IOSFont)_config.Font).InnerFont,
                            ParagraphStyle = style,
                            ForegroundColor = color
                        }, range);

                        using (CTFramesetter frameSetter = new CTFramesetter(str))
                        using (CGPath path = new CGPath())
                        {
                            path.AddRect(new CGRect(x, y, _maxWidth, _height));
                            using (var frame = frameSetter.GetFrame(range, path, null))
                            {
                                frame.Draw(_context);
                            }
                        }
                    }
                }
            }
        }

        private CTTextAlignment align()
        {
            switch (_config.Alignment)
            { 
                case Alignment.TopLeft:
                case Alignment.MiddleLeft:
                case Alignment.BottomLeft:
                    return CTTextAlignment.Left;
                case Alignment.TopCenter:
                case Alignment.MiddleCenter:
                case Alignment.BottomCenter:
                    return CTTextAlignment.Center;
                default:
                    return CTTextAlignment.Right;
            }
        }
    }
}
