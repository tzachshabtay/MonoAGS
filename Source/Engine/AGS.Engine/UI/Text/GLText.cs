using System;
using OpenTK.Graphics.OpenGL;
using System.Diagnostics;
using AGS.API;
using System.Text;

namespace AGS.Engine
{
	public class GLText
	{
		private string _text;
		private int _maxWidth;
		private ITextConfig _config;
		private IBitmap _bitmap;
		private int _texture;
		private bool _renderChanged;
		private BitmapPool _bitmapPool;
		private AGS.API.SizeF _baseSize;
        private int _caretPosition;
        private float _spaceWidth;
        private bool _cropText, _renderCaret;

        /// <summary>
        /// The factor in which the text will be rendered (and then will be downscaled to match the resolution so it would look sharper)
        /// </summary>
        public static int TextResolutionFactorX = 1;
        /// <summary>
        /// The factor in which the text will be rendered (and then will be downscaled to match the resolution so it would look sharper)
        /// </summary>
        public static int TextResolutionFactorY = 1;
        public static int TextResolutionWidth { get { return AGSGame.Game.VirtualResolution.Width * TextResolutionFactorX; } }
        public static int TextResolutionHeight { get { return AGSGame.Game.VirtualResolution.Height * TextResolutionFactorY; } }

        public GLText (BitmapPool pool, string text = "", int maxWidth = int.MaxValue)
		{
			this._maxWidth = maxWidth;
			this._text = text;
			this._bitmapPool = pool;            
			_texture = createTexture ();
			_config = new AGSTextConfig ();

			drawToBitmap();
		}

		public static AGS.API.SizeF EmptySize = new AGS.API.SizeF (0f, 0f);

		public int Texture { get { return _texture; } }

		public int BitmapWidth { get { return _bitmap.Width / 1; } }
		public int BitmapHeight { get { return _bitmap.Height / 1; } }
		public float Width { get; private set; }
		public float Height { get; private set; }        

		public void SetProperties(AGS.API.SizeF baseSize, string text = null, ITextConfig config = null, int? maxWidth = null, 
            int caretPosition = 0, bool renderCaret = false, bool? cropText = null)
		{
			bool changeNeeded = 
				(text != null && text != _text)
				|| (config != null && config != _config)
				|| (maxWidth != null && maxWidth.Value != _maxWidth)
				|| !baseSize.Equals(_baseSize)
                || _caretPosition != caretPosition
                || _renderCaret != renderCaret
                || (cropText != null && cropText.Value != _cropText);
			if (!changeNeeded) return;

			_text = text;
            if (config != null && config != _config)
            {
                _config = config;
                _spaceWidth = measureSpace();
            }
			if (maxWidth != null) _maxWidth = maxWidth.Value;
            if (cropText != null) _cropText = cropText.Value;
			_baseSize = baseSize;
            _caretPosition = caretPosition;
            _renderCaret = renderCaret;

			drawToBitmap();
		}

		public void Destroy()
		{
			IBitmap bitmap = _bitmap;
			if (bitmap != null) _bitmapPool.Release(bitmap);
			//todo: uncomment and remove from our textures map
			//GL.DeleteTexture (texture);
		}

		public void Refresh()
		{            
			if (_renderChanged)
			{
				_renderChanged = false;
				uploadBitmapToOpenGl ();
			}
		}

		private int createTexture()
		{
			int text_texture = GL.GenTexture();
			GL.BindTexture(TextureTarget.Texture2D, text_texture);
			GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)All.Linear);
			GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)All.Linear);
			//GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, 1024, 1024, 0,
			//	OpenTK.Graphics.OpenGL.PixelFormat.Bgra, PixelType.UnsignedByte, IntPtr.Zero); // just allocate memory, so we can update efficiently using TexSubImage2D
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
                newTextSize = config.Font.MeasureString(newText, int.MaxValue);                
            }
            text = result;
            return prevTextSize;
        }

        private void drawToBitmap()
        {
            string originalText = _text ?? "";
            string text = _text;

            var config = AGSTextConfig.ScaleConfig(_config, TextResolutionFactorX);
            int maxWidth = _maxWidth == int.MaxValue ? _maxWidth : _maxWidth * TextResolutionFactorX;
            SizeF originalTextSize = config.Font.MeasureString(text, _cropText ? int.MaxValue : maxWidth);
            SizeF textSize = originalTextSize;
            if (_cropText && textSize.Width > maxWidth)
            {
                textSize = cropText(maxWidth, config, ref text);
            }

            float widthOffset = Math.Max(config.OutlineWidth, Math.Abs(config.ShadowOffsetX));
            float heightOffset = Math.Max(config.OutlineWidth, Math.Abs(config.ShadowOffsetY));
            float widthF = textSize.Width + widthOffset + config.PaddingLeft + config.PaddingRight;
            float heightF = textSize.Height + heightOffset + config.PaddingTop + config.PaddingBottom;
            SizeF baseSize = new SizeF(_baseSize.Width == EmptySize.Width ? widthF : _baseSize.Width * TextResolutionFactorX,
                _baseSize.Height == EmptySize.Height ? heightF : _baseSize.Height * TextResolutionFactorY);

            Width = (widthF / GLText.TextResolutionFactorX);
            Height = (heightF / GLText.TextResolutionFactorY);
            int bitmapWidth = MathUtils.GetNextPowerOf2(Math.Max((int)widthF + 1, (int)baseSize.Width + 1));
            int bitmapHeight = MathUtils.GetNextPowerOf2(Math.Max((int)heightF + 1, (int)baseSize.Height + 1));
            IBitmap bitmap = _bitmap;
            if (bitmap == null || bitmap.Width != bitmapWidth || bitmap.Height != bitmapHeight)
            {
                if (bitmap != null) _bitmapPool.Release(bitmap);
                _bitmap = _bitmapPool.Acquire(bitmapWidth, bitmapHeight);
                bitmap = _bitmap;
            }
            IBitmapTextDraw textDraw = bitmap.GetTextDraw();
            using (var context = textDraw.CreateContext())
            {
                textDraw.DrawText(text, config, textSize, baseSize, maxWidth, (int)heightF, 0f);
                drawCaret(originalText, textSize, baseSize, textDraw, config, maxWidth);
            }

            _renderChanged = true;
		}

        private void drawCaret(string text, SizeF textSize, SizeF baseSize, IBitmapTextDraw textDraw, ITextConfig config, int maxWidth)
        {
            if (!_renderCaret) return;
            var caretPosition = _caretPosition;            
            
            if (caretPosition > text.Length) caretPosition = text.Length;
            string untilCaret = text.Substring(0, caretPosition);
            AGS.API.SizeF caretOffset = config.Font.MeasureString(untilCaret, maxWidth);
            float spaceOffset = 0f;
            if (untilCaret.EndsWith(" ")) spaceOffset = _spaceWidth * (untilCaret.Length - untilCaret.TrimEnd().Length);
            textDraw.DrawText("|", config, textSize, baseSize, maxWidth, (int)Height, caretOffset.Width + spaceOffset - 1f);            
        }

        private float measureSpace()
        {
            //hack to measure the size of spaces. For some reason MeasureString returns bad results when string ends with a space.
            IFont font = Hooks.FontLoader.LoadFont(_config.Font.FontFamily, _config.Font.SizeInPoints * TextResolutionFactorX, _config.Font.Style);
            return font.MeasureString(" a").Width - font.MeasureString("a").Width;
        }
			
		private void uploadBitmapToOpenGl()
		{
			try
			{
				// Upload the Bitmap to OpenGL.
				// Do this only when text changes.
				_bitmap.LoadTexture(_texture);
			}
			catch (InvalidOperationException e) 
			{
				Debug.WriteLine (e.ToString ());
				throw;
			}
		}
	}
}

