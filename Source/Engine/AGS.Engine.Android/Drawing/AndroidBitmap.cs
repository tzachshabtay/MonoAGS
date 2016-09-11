using AGS.API;
using Android.Graphics;
using OpenTK.Graphics.OpenGL;
using System.IO;

namespace AGS.Engine.Android
{
	public class AndroidBitmap : IBitmap
	{
		private Bitmap _bitmap;

		public AndroidBitmap(Bitmap bitmap)
		{
			_bitmap = bitmap;
		}

		#region IBitmap implementation

		public void Clear()
		{
			_bitmap.EraseColor(Colors.White.Convert());
		}

		public AGS.API.Color GetPixel(int x, int y)
		{
			return AGS.API.Color.FromHexa((uint)_bitmap.GetPixel(x, y));
		}

		public void MakeTransparent(AGS.API.Color color)
		{
			global::Android.Graphics.Color c = new global::Android.Graphics.Color(color.R, color.G, color.B, color.A);
			global::Android.Graphics.Color transparent = global::Android.Graphics.Color.Transparent;
			using (FastBitmap fastBitmap = new FastBitmap (_bitmap))
			{
				for (int x = 0; x < _bitmap.Width; x++)
				{
					for (int y = 0; y < _bitmap.Height; y++)
					{
						if (fastBitmap.GetPixel(x, y) == c)
							fastBitmap.SetPixel(x, y, c);
					}
				}
			}
		}

		public void LoadTexture(int? textureToBind)
		{
			var scan0 = _bitmap.LockPixels();

			if (textureToBind != null) 
				GL.BindTexture(TextureTarget.Texture2D, textureToBind.Value);

			GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, Width, Height, 0,
				OpenTK.Graphics.OpenGL.PixelFormat.Bgra, PixelType.UnsignedByte, scan0);
			_bitmap.UnlockPixels();
		}

		public IBitmap ApplyArea(IArea area)
		{
			//todo: performance can be improved by only creating a bitmap the size of the area, and not the entire background.
			//This will require to change the rendering as well to offset the location
			byte zero = (byte)0;
			Bitmap output = Bitmap.CreateBitmap(Width, Height, Bitmap.Config.Argb8888);
			using (FastBitmap inBmp = new FastBitmap (_bitmap))
			{
				using (FastBitmap outBmp = new FastBitmap (output, true))
				{
					for (int y = 0; y < Height; y++)
					{
						int bitmapY = Height - y - 1;
						for (int x = 0; x < Width; x++)
						{
							global::Android.Graphics.Color color = inBmp.GetPixel(x, bitmapY);
							byte alpha = area.IsInArea(new AGS.API.PointF(x, y)) ? color.A : zero;
							outBmp.SetPixel(x, bitmapY, new global::Android.Graphics.Color(color.R, color.G, color.B, alpha));
						}
					}
				}
			}

			return new AndroidBitmap(output);
		}

		public IBitmap Crop(AGS.API.Rectangle cropRect)
		{
			return new AndroidBitmap(Bitmap.CreateBitmap(_bitmap, cropRect.X, cropRect.Y, cropRect.Width, cropRect.Height)); //todo: improve performance by using FastBitmap
		}

		public IMask CreateMask(IGameFactory factory, string path, bool transparentMeansMasked = false, 
			AGS.API.Color? debugDrawColor = null, string saveMaskToFile = null, string id = null)
		{
			Bitmap debugMask = null;
			FastBitmap debugMaskFast = null;
			if (saveMaskToFile != null || debugDrawColor != null)
			{
				debugMask = Bitmap.CreateBitmap (Width, Height, Bitmap.Config.Argb8888);
				debugMaskFast = new FastBitmap (debugMask, true);
			}

			bool[][] mask = new bool[Width][];
			global::Android.Graphics.Color drawColor = debugDrawColor != null ? debugDrawColor.Value.Convert() : global::Android.Graphics.Color.Black;

			using (FastBitmap bitmapData = new FastBitmap (_bitmap))
			{
				for (int x = 0; x < Width; x++)
				{
					for (int y = 0; y < Height; y++)
					{
						var pixelColor = bitmapData.GetPixel(x, y);

						bool masked = pixelColor.A == 255;
						if (transparentMeansMasked)
							masked = !masked;

						if (mask[x] == null)
							mask[x] = new bool[Height];
						mask[x][Height - y - 1] = masked;

						if (debugMask != null)
						{
							debugMaskFast.SetPixel(x, y, masked ? drawColor : global::Android.Graphics.Color.Transparent);
						}
					}
				}
			}

			if (debugMask != null)
				debugMaskFast.Dispose();

			//Save the duplicate
			if (saveMaskToFile != null)
			{
				using (Stream stream = Hooks.FileSystem.Create(saveMaskToFile))
				{
					debugMask.Compress(global::Android.Graphics.Bitmap.CompressFormat.Png, 100, stream);
				}
			}	

			IObject debugDraw = null;
			if (debugDrawColor != null)
			{
				debugDraw = factory.Object.GetObject(id ?? path ?? "Mask Drawable");
				debugDraw.Image = factory.Graphics.LoadImage(new AndroidBitmap(debugMask), null, path);
				debugDraw.Anchor = new AGS.API.PointF ();
			}

			return new AGSMask (mask, debugDraw);
		}

		public IBitmapTextDraw GetTextDraw()
		{
            return new AndroidBitmapTextDraw(_bitmap);
		}

        public void SaveToFile(string path)
        {
            using (Stream stream = new FileStream(path, FileMode.OpenOrCreate, FileAccess.Write))
                _bitmap.Compress(Bitmap.CompressFormat.Png, 85, stream);
        }

		public int Width { get { return _bitmap.Width; } }

		public int Height { get { return _bitmap.Height; } }

		#endregion
	}
}

