using System;
using System.Drawing;
using AGS.API;
using System.Diagnostics;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;

namespace AGS.Engine
{
	public class WalkBehindBitmapCreator
	{
		public WalkBehindBitmapCreator()
		{
		}

		public Bitmap Create(IArea area, Bitmap bg)
		{
			//todo: performance can be improved by only creating a bitmap the size of the area, and not the entire background.
			//This will require to change the rendering as well to offset the location
			byte zero = (byte)0;
			Bitmap output = new Bitmap(bg.Width, bg.Height, PixelFormat.Format32bppArgb);
			using (FastBitmap inBmp = new FastBitmap (bg, ImageLockMode.ReadOnly))
			{
				using (FastBitmap outBmp = new FastBitmap (output, ImageLockMode.WriteOnly, true))
				{
					for (int y = 0; y < bg.Height; y++)
					{
						int bitmapY = bg.Height - y - 1;
						for (int x = 0; x < bg.Width; x++)
						{
							Color color = inBmp.GetPixel(x, bitmapY);
							byte alpha = area.IsInArea(new AGSPoint(x, y)) ? color.A : zero;
							outBmp.SetPixel(x, bitmapY, Color.FromArgb(alpha, color));
						}
					}
				}
			}

			return output;
		}
	}
}

