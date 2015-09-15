using System;
using OpenTK;
using System.Drawing;
using OpenTK.Graphics.OpenGL;
using System.Drawing.Imaging;
using System.Diagnostics;
using System.Drawing.Text;

namespace AGS.Engine
{
	public class GLText
	{
		private string _text;
		private int _maxWidth;
		private Font _font = new Font(SystemFonts.DefaultFont.FontFamily, 14f, FontStyle.Regular);
		private Bitmap _bitmap;
		private int _texture;
		private bool _renderChanged;
		private BitmapPool _bitmapPool;

		public GLText (BitmapPool pool, string text = "", int maxWidth = int.MaxValue)
		{
			this._maxWidth = maxWidth;
			this._text = text;
			this._bitmapPool = pool;
			_texture = createTexture ();

			drawToBitmap ();

			/*game.Resize += (sender, e) => 
			{
				// Ensure Bitmap and texture match window size
				text_bmp.Dispose();
				text_bmp = createBitmap();

				GL.BindTexture(TextureTarget.Texture2D, texture);
				GL.TexSubImage2D(TextureTarget.Texture2D, 0, 0, 0, text_bmp.Width, text_bmp.Height,
					OpenTK.Graphics.OpenGL.PixelFormat.Bgra, PixelType.UnsignedByte, IntPtr.Zero);
			};*/
		}

		public string Text { get { return _text; } set { SetBatch(value); } }

		public int MaxWidth { get { return _maxWidth; } set { SetBatch(maxWidth: value); } }

		public Font Font { get { return _font; } set { SetBatch(font: value); } }

		public int Texture { get { return _texture; } }

		public void SetBatch(string text = null, Font font = null, int? maxWidth = null)
		{
			bool changeNeeded = (text != null && text != _text) || (font != null && !font.Equals(_font))
			                    || (maxWidth != null && maxWidth.Value != _maxWidth);
			if (!changeNeeded) return;

			_text = text;
			if (font != null) _font = font;
			if (maxWidth != null) _maxWidth = maxWidth.Value;
			drawToBitmap();
		}

		public void Destroy()
		{
			Bitmap bitmap = _bitmap;
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

		private void drawToBitmap ()
		{
			SizeF textSize = _text.Measure(_font, _maxWidth);
			int width = MathUtils.GetNextPowerOf2((int)textSize.Width + 1);
			int height = MathUtils.GetNextPowerOf2((int)textSize.Height + 1);
			Bitmap bitmap = _bitmap;
			if (bitmap == null || bitmap.Width != width || bitmap.Height != height)
			{
				if (bitmap != null) _bitmapPool.Release(bitmap);
				_bitmap = _bitmapPool.Acquire(width, height);
			}

			using (Graphics gfx = Graphics.FromImage (_bitmap)) 
			{
				gfx.Clear(Color.Transparent);
				gfx.TextRenderingHint = TextRenderingHint.AntiAlias;
				gfx.DrawString (_text, _font, Brushes.White, 0f, 0f);
			}

			_renderChanged = true;
		}

		private void uploadBitmapToOpenGl()
		{
			try
			{
				// Upload the Bitmap to OpenGL.
				// Do this only when text changes.
				BitmapData data = _bitmap.LockBits(new Rectangle(0, 0, _bitmap.Width, _bitmap.Height), ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
				GL.BindTexture(TextureTarget.Texture2D, _texture);
				GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, data.Width, data.Height, 0,
					OpenTK.Graphics.OpenGL.PixelFormat.Bgra, PixelType.UnsignedByte, data.Scan0); 
				_bitmap.UnlockBits(data);
			}
			catch (InvalidOperationException e) 
			{
				Debug.WriteLine (e.ToString ());
				throw;
			}
		}
	}
}

