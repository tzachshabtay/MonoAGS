using System;
using System.Diagnostics;
using System.Text;
using AGS.API;

namespace AGS.Engine
{
    public class GLText : IDisposable
    {
        private string _text;
        private int _maxWidth;
        private ITextConfig _config;
        private int _texture;
        private bool _renderChanged;
        private BitmapPool _bitmapPool;
        private SizeF _baseSize;
        private int _caretPosition;
        private float _spaceWidth;
        private bool _cropText, _renderCaret, _measureOnly;
        private readonly IGraphicsBackend _graphics;
        private readonly IFontLoader _fonts;
        private readonly IRenderMessagePump _messagePump;
        private readonly bool _alwaysMeasureOnly;
        private PointF _scaleUp = new PointF(TextResolutionFactorX, TextResolutionFactorY);
        private PointF _scaleDown = AGSModelMatrixComponent.NoScaling;

        private class DrawInfo
        {
            public AGSTextConfig Config { get; set; }
            public int BitmapWidth { get; set; }
            public int BitmapHeight { get; set; }
            public string Text { get; set; }
            public string OriginalText { get; set; }
            public SizeF TextSize { get; set; }
            public SizeF BaseSize { get; set; }
            public float HeightF { get; set; }
            public int MaxWidth { get; set; }
        }
        private DrawInfo _draw = new DrawInfo();

        /// <summary>
        /// The factor in which the text will be rendered (and then will be downscaled to match the resolution so it would look sharper)
        /// </summary>
        public static int TextResolutionFactorX = 1;
        /// <summary>
        /// The factor in which the text will be rendered (and then will be downscaled to match the resolution so it would look sharper)
        /// </summary>
        public static int TextResolutionFactorY = 1;

        public GLText(IGraphicsBackend graphics, IRenderMessagePump messagePump, IFontLoader fonts, IFont defaultFont, BitmapPool pool, 
                      bool alwaysMeasureOnly, string text = "", int maxWidth = int.MaxValue)
        {
            _messagePump = messagePump;
            _fonts = fonts;
            _graphics = graphics;
            _alwaysMeasureOnly = alwaysMeasureOnly;
            _maxWidth = maxWidth;
            _text = text;
            _bitmapPool = pool;
            _config = new AGSTextConfig(font: defaultFont);

            prepareBitmapDraw();
        }

        ~GLText()
        {
            disposeTexture();
        }

        public static SizeF EmptySize = new SizeF(0f, 0f);

        public int Texture => _texture;

        public int BitmapWidth { get; private set; }
        public int BitmapHeight { get; private set; }
        public float Width { get; private set; }
        public float Height { get; private set; }

        public bool SetProperties(SizeF baseSize, string text = null, ITextConfig config = null, int? maxWidth = null,
              PointF? scaleUp = null, PointF? scaleDown = null, int caretPosition = 0, bool renderCaret = false,
              bool cropText = false, bool measureOnly = false)
        {
            bool configIsDifferent = config != null && !config.Equals(_config);
            bool changeNeeded =
                (text != null && text != _text)
                || configIsDifferent
                || (maxWidth != null && maxWidth.Value != _maxWidth)
                || !baseSize.Equals(_baseSize)
                || _caretPosition != caretPosition
                || _renderCaret != renderCaret
                || cropText != _cropText
                || measureOnly != _measureOnly
                || (scaleUp != null && !scaleUp.Value.Equals(_scaleUp))
                || (scaleDown != null && !scaleDown.Value.Equals(_scaleDown));

            if (!changeNeeded) return false;

            _text = text;
            if (configIsDifferent)
            {
                _config = AGSTextConfig.Clone(config);
                _spaceWidth = measureSpace();
            }
            if (maxWidth != null) _maxWidth = maxWidth.Value;
            _cropText = cropText;
            _measureOnly = measureOnly;
            if (scaleUp != null) _scaleUp = scaleUp.Value;
            if (scaleDown != null) _scaleDown = scaleDown.Value;

            _baseSize = baseSize;
            _caretPosition = caretPosition;
            _renderCaret = renderCaret;

            prepareBitmapDraw();

            return true;
        }

        public void Dispose()
        {
            disposeTexture();
            GC.SuppressFinalize(this);
        }

        public void Refresh()
        {
            if (_renderChanged)
            {
                _renderChanged = false;
                uploadBitmapToOpenGl();
            }
        }

        private int createTexture()
        {
            int text_texture = _graphics.GenTexture();
            _graphics.BindTexture2D(text_texture);
            _graphics.SetTextureMagFilter(ScaleUpFilters.Linear);
            _graphics.SetTextureMinFilter(ScaleDownFilters.Linear);
            //GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, 1024, 1024, 0,
            //    OpenTK.Graphics.OpenGL.PixelFormat.Bgra, PixelType.UnsignedByte, IntPtr.Zero); // just allocate memory, so we can update efficiently using TexSubImage2D
            return text_texture;
        }

        private SizeF cropText(int maxWidth, ITextConfig config, ref string text)
        {
            var caretPosition = _caretPosition;
            if (caretPosition >= text.Length) caretPosition = text.Length - 1;
            SizeF newTextSize = new SizeF();
            SizeF prevTextSize = newTextSize;
            StringBuilder textBuilder = new StringBuilder();
            string newText = "";
            string result = "";
            int currentPosition = caretPosition;
            while (newTextSize.Width < maxWidth)
            {
                result = newText;
                prevTextSize = newTextSize;
                if (currentPosition > caretPosition)
                {
                    textBuilder.Append(text[currentPosition]);
                    currentPosition++;
                }
                else
                {
                    textBuilder.Insert(0, text[currentPosition]);
                    currentPosition--;
                    if (currentPosition < 0) currentPosition = caretPosition + 1;
                }
                newText = textBuilder.ToString();
                newTextSize = config.Font.MeasureString(newText);
            }
            text = result;
            return prevTextSize;
        }

        private void prepareBitmapDraw()
        {
            string originalText = _text ?? "";
            string text = _text;

            var config = AGSTextConfig.ScaleConfig(_config, _scaleUp.X);
            int maxWidth = _maxWidth == int.MaxValue ? _maxWidth : (int)(_maxWidth * _scaleUp.X - config.PaddingLeft - config.PaddingRight);
            string textToMeasure = text;
            if (_renderCaret && textToMeasure == "") textToMeasure = "|";
            SizeF originalTextSize = config.Font.MeasureString(textToMeasure, _cropText ? int.MaxValue : maxWidth);
            SizeF textSize = originalTextSize;
            if (_cropText && textSize.Width > maxWidth)
            {
                textSize = cropText(maxWidth, config, ref text);
            }

            float widthOffset = Math.Max(config.OutlineWidth, Math.Abs(config.ShadowOffsetX));
            float heightOffset = Math.Max(config.OutlineWidth, Math.Abs(config.ShadowOffsetY));
            float widthF = textSize.Width + widthOffset + config.PaddingLeft + config.PaddingRight;
            float heightF = textSize.Height + heightOffset + config.PaddingTop + config.PaddingBottom;
            SizeF baseSize = new SizeF(_baseSize.Width == EmptySize.Width ? widthF : _baseSize.Width * _scaleUp.X,
                                       _baseSize.Height == EmptySize.Height ? heightF : _baseSize.Height * _scaleUp.Y);

            Width = (widthF / _scaleUp.X);
            Height = (heightF / _scaleUp.Y);
            int bitmapWidth = MathUtils.GetNextPowerOf2(Math.Max((int)(widthF) + 1, (int)(baseSize.Width) + 1));
            int bitmapHeight = MathUtils.GetNextPowerOf2(Math.Max((int)(heightF) + 1, (int)(baseSize.Height) + 1));
            BitmapWidth = (int)(bitmapWidth / _scaleDown.X);
            BitmapHeight = (int)(bitmapHeight / _scaleDown.Y);
            if (_measureOnly || _alwaysMeasureOnly) return;

            _draw.BitmapWidth = bitmapWidth;
            _draw.BitmapHeight = bitmapHeight;
            _draw.BaseSize = baseSize;
            _draw.TextSize = textSize;
            _draw.HeightF = heightF;
            _draw.Text = text;
            _draw.OriginalText = originalText;
            _draw.MaxWidth = maxWidth;
            _draw.Config = config;

            _renderChanged = true;
        }

        private void drawCaret(string text, SizeF textSize, float heightF, SizeF baseSize, IBitmapTextDraw textDraw, ITextConfig config, int maxWidth)
        {
            if (!_renderCaret) return;
            var caretPosition = _caretPosition;

            if (caretPosition > text.Length) caretPosition = text.Length;
            string untilCaret = text.Substring(0, caretPosition);
            SizeF caretOffset = config.Font.MeasureString(untilCaret, maxWidth);
            float spaceOffset = 0f;
            if (untilCaret.EndsWith(" ", StringComparison.Ordinal)) spaceOffset = _spaceWidth * (untilCaret.Length - untilCaret.TrimEnd().Length);
            textDraw.DrawText("|", config, textSize, baseSize, maxWidth, (int)heightF, caretOffset.Width + spaceOffset - 1f);
        }

        private float measureSpace()
        {
            //hack to measure the size of spaces. For some reason MeasureString returns bad results when string ends with a space.
            IFont font = _fonts.LoadFont(_config.Font.FontFamily, _config.Font.SizeInPoints * _scaleUp.X, _config.Font.Style);
            return font.MeasureString(" a").Width - font.MeasureString("a").Width;
        }

        private void uploadBitmapToOpenGl()
        {
            disposeTexture();
            _texture = createTexture();
            IBitmap bitmap = _bitmapPool.Acquire(_draw.BitmapWidth, _draw.BitmapHeight);
            if (bitmap == null) return;
            try
            {
                IBitmapTextDraw textDraw = bitmap.GetTextDraw();
                using (textDraw.CreateContext())
                {
                    textDraw.DrawText(_draw.Text, _draw.Config, _draw.TextSize, _draw.BaseSize, _draw.MaxWidth, (int)_draw.HeightF, 0f);
                    drawCaret(_draw.OriginalText, _draw.TextSize, _draw.HeightF, _draw.BaseSize, textDraw, _draw.Config, _draw.MaxWidth);
                }
                // Upload the Bitmap to OpenGL.
                // Do this only when text changes.
                bitmap.LoadTexture(_texture);
            }
            catch (InvalidOperationException e)
            {
                Debug.WriteLine(e.ToString());
                throw;
            }
            finally
            {
                releaseBitmap(bitmap);
            }
        }

        private void disposeTexture()
        {
            var texture = _texture;
            if (texture != 0) _messagePump.Post(_ => _graphics.DeleteTexture(texture), null);
        }

        private void releaseBitmap(IBitmap bitmap)
        {
            if (bitmap == null) return;
            try 
            { 
                _bitmapPool.Release(bitmap); 
            } 
            catch (ArgumentException e) 
            { 
                Debug.WriteLine(e.ToString()); 
            }
        }
    }
}
