using System;
using System.Drawing;
using API;
using System.Diagnostics;

namespace Engine
{
	public class WalkBehindBitmapCreator
	{
		public WalkBehindBitmapCreator()
		{
		}

		public Bitmap Create(IArea area, Bitmap bg)
		{
			//todo: Extremely naive implementation-> Performance can be improved by only creating mask-sized bitmap and/or by using LockBits with low level pointer math
			Bitmap result = new Bitmap (bg.Width, bg.Height);
			for (int x = 0; x < bg.Width; x++)
			{
				for (int y = 0; y < bg.Height; y++)
				{
					Color color = Color.Transparent;
					int bitmapY = bg.Height - y - 1;
					if (area.IsInArea(new AGSPoint(x, y)))
					{
						color = bg.GetPixel(x, bitmapY);
					}
					result.SetPixel(x, bitmapY, color);
				}
			}
			return result;
		}
	}
}

