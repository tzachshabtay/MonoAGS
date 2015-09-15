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
			const int bpp = 4;
			int byteCount = bg.Width * bg.Height * bpp;

			var inBytes  = new byte[byteCount];
			var outBytes = new byte[byteCount];

			var inBmpData = bg.LockBits(new Rectangle(0, 0, bg.Width, bg.Height), ImageLockMode.ReadOnly, 
				PixelFormat.Format32bppArgb);

			Marshal.Copy(inBmpData.Scan0, inBytes, 0, byteCount);

			for (int y = 0; y < bg.Height; y++)
			{
				int bitmapY = bg.Height - y - 1;
				for (int x = 0; x < bg.Width; x++)
				{
					int offset = (bg.Width * bitmapY + x) * bpp;

					byte blue  = inBytes[offset];
					byte green = inBytes[offset + 1];
					byte red   = inBytes[offset + 2];
					byte alpha = area.IsInArea(new AGSPoint(x, y)) ? inBytes[offset + 3] : zero;

					outBytes[offset]     = blue;
					outBytes[offset + 1] = green;
					outBytes[offset + 2] = red;
					outBytes[offset + 3] = alpha;
				}
			}

			bg.UnlockBits(inBmpData);

			Bitmap output = new Bitmap(bg.Width, bg.Height, PixelFormat.Format32bppArgb);

			var outBmpData = output.LockBits(new Rectangle(0, 0, output.Width, output.Height), ImageLockMode.WriteOnly, output.PixelFormat);

			Marshal.Copy(outBytes, 0, outBmpData.Scan0, outBytes.Length);

			output.UnlockBits(outBmpData);

			return output;
		}
	}
}

