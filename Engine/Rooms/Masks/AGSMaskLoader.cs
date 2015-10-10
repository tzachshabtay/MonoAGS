using System;
using AGS.API;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;

namespace AGS.Engine
{
	public class AGSMaskLoader : IMaskLoader
	{
		private IGameFactory _factory;

		public AGSMaskLoader(IGameFactory factory)
		{
			_factory = factory;
		}

		#region IMaskLoader implementation

		public IMask Load(string path, bool transparentMeansMasked = false, 
			Color? debugDrawColor = null, string saveMaskToFile = null)
		{
			Bitmap image = (Bitmap)Image.FromFile(path); 
			return Load(image, transparentMeansMasked, debugDrawColor, saveMaskToFile);
		}

		public IMask Load(Bitmap image, bool transparentMeansMasked = false, 
			Color? debugDrawColor = null, string saveMaskToFile = null)
		{
			Bitmap debugMask = null;
			FastBitmap debugMaskFast = null;
			if (saveMaskToFile != null || debugDrawColor != null)
			{
				debugMask = new Bitmap (image.Width, image.Height);
				debugMaskFast = new FastBitmap (debugMask, ImageLockMode.WriteOnly, true);
			}
				
			bool[][] mask = new bool[image.Width][];
			Color drawColor = debugDrawColor.HasValue ? debugDrawColor.Value : Color.Black;

			using (FastBitmap bitmapData = new FastBitmap (image, ImageLockMode.ReadOnly))
			{
				for (int x = 0; x < image.Width; x++)
				{
					for (int y = 0; y < image.Height; y++)
					{
						var pixelColor = bitmapData.GetPixel(x, y);

						bool masked = pixelColor.A == 255;
						if (transparentMeansMasked)
							masked = !masked;

						if (mask[x] == null)
							mask[x] = new bool[image.Height];
						mask[x][image.Height - y - 1] = masked;

						if (debugMask != null)
						{
							debugMaskFast.SetPixel(x, y, masked ? drawColor : Color.Transparent);
						}
					}
				}
			}
				
			if (debugMask != null)
				debugMaskFast.Dispose();

			//Save the duplicate
			if (saveMaskToFile != null)
				debugMask.Save (saveMaskToFile);

			IObject debugDraw = null;
			if (debugDrawColor != null)
			{
				debugDraw = _factory.Object.GetObject();
				debugDraw.Image = _factory.Graphics.LoadImage(debugMask);
				debugDraw.Anchor = new AGSPoint ();
			}

			return new AGSMask (mask, debugDraw);
		}
			
		#endregion
	}
}

