using AGS.API;
using System.Drawing;
using System.Drawing.Imaging;
using System;
using System.Collections.Generic;

namespace AGS.Engine.Desktop
{
    public class DesktopBitmap : IBitmap, IDisposable
	{
		private readonly Bitmap _bitmap;
        private readonly IGraphicsBackend _graphics;

        public DesktopBitmap(Bitmap bitmap, IGraphicsBackend graphics)
		{
			_bitmap = bitmap;
            Width = bitmap.Width;
            Height = bitmap.Height;
            _graphics = graphics;
		}

        ~DesktopBitmap()
        {
            dispose(false);
        }

		#region IBitmap implementation

		public void Clear()
		{
			_bitmap.Clear();
		}

		public AGS.API.Color GetPixel(int x, int y)
		{
			return _bitmap.GetPixel(x, y).Convert();
		}

        public void SetPixel(AGS.API.Color color, int x, int y)
        {
            _bitmap.SetPixel(x, y, color.Convert());
        }

        public void SetPixels(AGS.API.Color color, List<API.Point> points)
        {
            using (FastBitmap bmp = new FastBitmap(_bitmap, ImageLockMode.WriteOnly))
            {
                foreach (var point in points)
                {
                    bmp.SetPixel(point.X, point.Y, color.Convert());
                }
            }
        }

		public void MakeTransparent(AGS.API.Color color)
		{
			_bitmap.MakeTransparent(color.Convert());
		}

		public void LoadTexture(int? textureToBind)
		{
			BitmapData data = _bitmap.LockBits(new System.Drawing.Rectangle(0, 0, Width, Height),
				ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);

            if (textureToBind != null)
                _graphics.BindTexture2D(textureToBind.Value);

            _graphics.TexImage2D(Width, Height, data.Scan0);
			_bitmap.UnlockBits(data);
		}

		public IBitmap ApplyArea(IAreaComponent area)
		{
			//todo: performance can be improved by only creating a bitmap the size of the area, and not the entire background.
			//This will require to change the rendering as well to offset the location
			byte zero = (byte)0;
			Bitmap output = new Bitmap(Width, Height, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
			using (FastBitmap inBmp = new FastBitmap (_bitmap, ImageLockMode.ReadOnly))
			{
				using (FastBitmap outBmp = new FastBitmap (output, ImageLockMode.WriteOnly, true))
				{
					for (int y = 0; y < Height; y++)
					{
						int bitmapY = Height - y - 1;
						for (int x = 0; x < Width; x++)
						{
							System.Drawing.Color color = inBmp.GetPixel(x, bitmapY);
							byte alpha = area.IsInArea(new AGS.API.PointF(x, y)) ? color.A : zero;
							outBmp.SetPixel(x, bitmapY, System.Drawing.Color.FromArgb(alpha, color));
						}
					}
				}
			}

            return new DesktopBitmap(output, _graphics);
		}

		public IBitmap Crop(AGS.API.Rectangle cropRect)
		{
            return new DesktopBitmap(_bitmap.Clone (cropRect.Convert(), _bitmap.PixelFormat), _graphics); //todo: improve performance by using FastBitmap
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
				debugMask = new Bitmap (bitmapWidth, bitmapHeight);
				debugMaskFast = new FastBitmap (debugMask, ImageLockMode.WriteOnly, true);
			}

			bool[][] mask = new bool[bitmapWidth][];
			System.Drawing.Color drawColor = debugDrawColor != null ? debugDrawColor.Value.Convert() : System.Drawing.Color.Black;

			using (FastBitmap bitmapData = new FastBitmap (_bitmap, ImageLockMode.ReadOnly))
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

						debugMaskFast?.SetPixel(x, y, masked ? drawColor : System.Drawing.Color.Transparent);
					}
				}
			}

			debugMaskFast?.Dispose();

			//Save the duplicate
			if (saveMaskToFile != null)
				debugMask.Save (saveMaskToFile);

			IObject debugDraw = null;
			if (debugDrawColor != null)
			{
				debugDraw = factory.Object.GetAdventureObject(id ?? path ?? "Mask Drawable");
                debugDraw.Image = factory.Graphics.LoadImage(new DesktopBitmap(debugMask, _graphics), null, path);
				debugDraw.Pivot = new AGS.API.PointF ();
			}

			return new AGSMask (mask, debugDraw);
		}

		public IBitmapTextDraw GetTextDraw()
		{
            return new DesktopBitmapTextDraw(_bitmap);
		}

        public void SaveToFile(string path)
        {
            _bitmap.Save(path, ImageFormat.Png);
        }

        public void Dispose()
        {
            dispose(true);
            GC.SuppressFinalize(this);
        }

        public int Width { get; private set; }

        public int Height { get; private set; }

        #endregion

        private void dispose(bool disposing)
        { 
            _bitmap?.Dispose();
        }
	}
}

