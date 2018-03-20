using AGS.API;
using Android.Graphics;
using System.IO;
using System;
using System.Collections.Generic;

namespace AGS.Engine.Android
{
	public class AndroidBitmap : IBitmap, IDisposable
	{
		private readonly Bitmap _bitmap;
        private readonly IGraphicsBackend _graphics;

        public AndroidBitmap(Bitmap bitmap, IGraphicsBackend graphics)
		{
			_bitmap = bitmap;
            Width = _bitmap.Width;
            Height = _bitmap.Height;
            _graphics = graphics;
		}

        ~AndroidBitmap()
        {
            dispose(false);
        }

		#region IBitmap implementation

		public void Clear()
		{
            _bitmap.EraseColor(global::Android.Graphics.Color.White);
		}

		public AGS.API.Color GetPixel(int x, int y)
		{
			return AGS.API.Color.FromHexa((uint)_bitmap.GetPixel(x, y));
		}

        public void SetPixel(AGS.API.Color color, int x, int y)
        {
            _bitmap.SetPixel(x, y, color.Convert());
        }

		public void SetPixels(AGS.API.Color color, List<API.Point> points)
		{
			using (FastBitmap bmp = new FastBitmap(_bitmap))
			{
				foreach (var point in points)
				{
					bmp.SetPixel(point.X, point.Y, color.Convert());
				}
			}
		}

		public void MakeTransparent(AGS.API.Color color)
		{
            global::Android.Graphics.Color c = new global::Android.Graphics.Color(color.R, color.G, color.B, color.A);
			global::Android.Graphics.Color transparent = global::Android.Graphics.Color.Transparent;
			using (FastBitmap fastBitmap = new FastBitmap (_bitmap))
			{
                for (int x = 0; x < Width; x++)
				{
                    for (int y = 0; y < Height; y++)
					{
						if (fastBitmap.GetPixel(x, y) == c)
                            fastBitmap.SetPixel(x, y, transparent);
					}
				}
			}
		}

		public void LoadTexture(int? textureToBind)
		{
			var scan0 = _bitmap.LockPixels();

            if (textureToBind != null)
                _graphics.BindTexture2D(textureToBind.Value);

            _graphics.TexImage2D(Width, Height, scan0);
			_bitmap.UnlockPixels();
		}

		public IBitmap ApplyArea(IAreaComponent area)
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

            return new AndroidBitmap(output, _graphics);
		}

		public IBitmap Crop(AGS.API.Rectangle cropRect)
		{
            return new AndroidBitmap(Bitmap.CreateBitmap(_bitmap, cropRect.X, cropRect.Y, cropRect.Width, cropRect.Height), _graphics); //todo: improve performance by using FastBitmap
		}

		public IMask CreateMask(IGameFactory factory, string path, bool transparentMeansMasked = false, 
			AGS.API.Color? debugDrawColor = null, string saveMaskToFile = null, string id = null)
		{
			Bitmap debugMask = null;
			FastBitmap debugMaskFast = null;
            int bitmapWidth = Width;
            int bitmapHeight = Height;
			if (saveMaskToFile != null || debugDrawColor != null)
			{
                debugMask = Bitmap.CreateBitmap (bitmapWidth, bitmapHeight, Bitmap.Config.Argb8888);
				debugMaskFast = new FastBitmap (debugMask, true);
			}

			bool[][] mask = new bool[bitmapWidth][];
			global::Android.Graphics.Color drawColor = debugDrawColor != null ? debugDrawColor.Value.Convert() : global::Android.Graphics.Color.Black;

			using (FastBitmap bitmapData = new FastBitmap (_bitmap))
			{
				for (int x = 0; x < bitmapWidth; x++)
				{
					for (int y = 0; y < bitmapHeight; y++)
					{
                        bool masked = bitmapData.IsOpaque(x, y);

						if (transparentMeansMasked)
							masked = !masked;

						if (mask[x] == null)
							mask[x] = new bool[bitmapHeight];
						mask[x][bitmapHeight - y - 1] = masked;

						debugMaskFast?.SetPixel(x, y, masked ? drawColor : global::Android.Graphics.Color.Transparent);
					}
				}
			}

			debugMaskFast?.Dispose();

			//Save the duplicate
			if (saveMaskToFile != null)
			{
                using (Stream stream = AGSGame.Device.FileSystem.Create(saveMaskToFile))
				{
					debugMask.Compress(global::Android.Graphics.Bitmap.CompressFormat.Png, 100, stream);
				}
			}	

			IObject debugDraw = null;
			if (debugDrawColor != null)
			{
				debugDraw = factory.Object.GetObject(id ?? path ?? "Mask Drawable");
                debugDraw.Image = factory.Graphics.LoadImage(new AndroidBitmap(debugMask, _graphics), null, path);
				debugDraw.Pivot = new AGS.API.PointF ();
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

        public int Width { get; private set; }

        public int Height { get; private set; }

        #endregion

        public void Dispose()
        {
            dispose(true);
            GC.SuppressFinalize(this);
        }

        private void dispose(bool disposing)
        {
            _bitmap?.Dispose();
        }
	}
}

