using System;
using OpenTK;
using System.Drawing;
using OpenTK.Graphics.OpenGL;
using System.Drawing.Imaging;

namespace AGS.Engine
{
	public class GLText
	{
		private string _text;
		private Bitmap _bitmap;
		private int _texture;
		private bool _textChanged;

		public GLText (/*GameWindow game,*/ string text)
		{
			this._text = text;
			_bitmap = createBitmap ();
			_texture = createTexture ();

			drawToBitmap ();
			uploadBitmapToOpenGl ();

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

		public bool Visible { get; set; }

		public string Text 
		{ 
			get { return _text; } 
			set 
			{ 
				if (_text == value)
					return;
				_text = value; 
				drawToBitmap();
				_textChanged = true;
			} 
		}

		public int Texture { get { return _texture; } }

		public void Destroy()
		{
			//todo: uncomment and remove from our textures map
			//GL.DeleteTexture (texture);
		}

		public void Refresh()
		{
			if (_textChanged)
			{
				_textChanged = false;
				uploadBitmapToOpenGl ();
			}
		}

		public void Render()
		{
			if (!Visible)
				return;
			Refresh();
			renderTexture();
		}

		private Bitmap createBitmap()
		{
			return new Bitmap (400, 40);
		}

		private int createTexture()
		{
			int text_texture = GL.GenTexture();
			GL.BindTexture(TextureTarget.Texture2D, text_texture);
			GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)All.Linear);
			GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)All.Linear);
			GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, _bitmap.Width, _bitmap.Height, 0,
				OpenTK.Graphics.OpenGL.PixelFormat.Bgra, PixelType.UnsignedByte, IntPtr.Zero); // just allocate memory, so we can update efficiently using TexSubImage2D
			return text_texture;
		}

		private void drawToBitmap ()
		{
			// Render text using System.Drawing.
			// Do this only when text changes.
			using (Graphics gfx = Graphics.FromImage (_bitmap)) 
			{
				gfx.Clear (Color.Transparent);
				gfx.DrawString (_text, SystemFonts.DefaultFont, Brushes.White, 0f, 0f);
			}
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
				Console.WriteLine (e.ToString ());
				throw;
			}
		}

		private void renderTexture()
		{
			GLUtils.DrawQuad (_texture, 350f, 0f, _bitmap.Width*2, _bitmap.Height*2, 1f, 1f, 1f, 1f, 1f);
		}
	}
}

