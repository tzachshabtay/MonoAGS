using System;
using OpenTK;

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

		public int BitmapWidth { get { return _bitmap.Width; } }
		public int BitmapHeight { get { return _bitmap.Height; } }
		public int Width { get; private set; }
		public int Height { get; private set; }        

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
            if (config != null)
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

        private SizeF cropText(ref string text)
        {
            var caretPosition = _caretPosition;
            if (caretPosition >= text.Length) caretPosition = text.Length - 1;
            SizeF newTextSize = new SizeF();
            SizeF prevTextSize = newTextSize;
            StringBuilder textBuilder = new StringBuilder();
            string newText = "";
            string result = "";
            int currentPosition = caretPosition;
            while (newTextSize.Width < _maxWidth)
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
                newTextSize = _config.Font.MeasureString(newText, int.MaxValue);                
            }
            text = result;
            return prevTextSize;
        }

        private void drawToBitmap ()
		{
            string originalText = _text ?? "";            
            string text = _text;
            SizeF originalTextSize = _config.Font.MeasureString(text, _cropText ? int.MaxValue : _maxWidth);
            SizeF textSize = originalTextSize;
            if (_cropText && textSize.Width > _maxWidth)
            {
                textSize = cropText(ref text);
            }

			float widthOffset = Math.Max(_config.OutlineWidth, Math.Abs(_config.ShadowOffsetX));
			float heightOffset = Math.Max(_config.OutlineWidth, Math.Abs(_config.ShadowOffsetY));
			float widthF = textSize.Width + widthOffset + _config.PaddingLeft + _config.PaddingRight;
			float heightF = textSize.Height + heightOffset + _config.PaddingTop + _config.PaddingBottom;
            SizeF baseSize = new SizeF(_baseSize.Width == EmptySize.Width ? widthF : _baseSize.Width,
                _baseSize.Height == EmptySize.Height ? heightF : _baseSize.Height);

            Width = (int)widthF + 2;
            Height = (int)heightF + 2;
            int bitmapWidth = MathUtils.GetNextPowerOf2(Math.Max(Width, (int)_baseSize.Width + 2));
			int bitmapHeight = MathUtils.GetNextPowerOf2(Math.Max(Height, (int)_baseSize.Height + 2));
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
                textDraw.DrawText(text, _config, textSize, baseSize, _maxWidth, Height, 0f);
                drawCaret(originalText, textSize, baseSize, textDraw);
            }
            
            _renderChanged = true;
		}

        private void drawCaret(string text, SizeF textSize, SizeF baseSize, IBitmapTextDraw textDraw)
        {
            if (!_renderCaret) return;
            var caretPosition = _caretPosition;            
            
            if (caretPosition > text.Length) caretPosition = text.Length;
            string untilCaret = text.Substring(0, caretPosition);
            AGS.API.SizeF caretOffset = _config.Font.MeasureString(untilCaret, _maxWidth);
            float spaceOffset = 0f;
            if (untilCaret.EndsWith(" ")) spaceOffset = _spaceWidth * (untilCaret.Length - untilCaret.TrimEnd().Length);
            textDraw.DrawText("|", _config, textSize, baseSize, _maxWidth, Height, caretOffset.Width + spaceOffset - 1f);            
        }

        private float measureSpace()
        {
            //hack to measure the size of spaces. For some reason MeasureString returns bad results when string ends with a space.
            IFont font = _config.Font;
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

