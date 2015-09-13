using System;
using API;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;

namespace Engine
{
	public class AGSMaskLoader : IMaskLoader
	{
		private IGraphicsFactory _factory;

		public AGSMaskLoader(IGraphicsFactory factory)
		{
			_factory = factory;
		}

		#region IMaskLoader implementation

		public IMask Load(string path, bool transparentMeansMasked = false, 
			Color? debugDrawColor = null, string saveMaskToFile = null)
		{
			Bitmap image = (Bitmap)Image.FromFile(path); 

			//Get the bitmap data
			var bitmapData = image.LockBits (
				new Rectangle (0, 0, image.Width, image.Height),
				ImageLockMode.ReadWrite, 
				image.PixelFormat
			);

			//Initialize an array for all the image data
			byte[] imageBytes = new byte[bitmapData.Stride * image.Height];

			//Copy the bitmap data to the local array
			//todo: This might require some more work (casting to int* to avoid little endianess issues),
			//see here: http://stackoverflow.com/questions/8104461/pixelformat-format32bppargb-seems-to-have-wrong-byte-order
			Marshal.Copy (bitmapData.Scan0, imageBytes, 0, imageBytes.Length);

			//Unlock the bitmap
			image.UnlockBits(bitmapData);

			//Find pixelsize
			int pixelSize = Image.GetPixelFormatSize(image.PixelFormat);

			// An example on how to use the pixels, lets make a copy
			int x = 0;
			int y = 0;
			Bitmap debugMask = null;
			if (saveMaskToFile != null || debugDrawColor != null)
				debugMask = new Bitmap (image.Width, image.Height);

			bool[][] mask = new bool[image.Width][];
			//Loop pixels
			for(int i=0;i<imageBytes.Length;i+=pixelSize/8)
			{
				//Copy the bits into a local array
				var pixelData = new byte[4];
				Array.Copy(imageBytes,i,pixelData,0,4);

				//Get the color of a pixel
				//todo: add more formats, currently assuming Format32bppArgb
				var pixelColor = Color.FromArgb(pixelData[3], pixelData [2], pixelData [1], pixelData [0]);

				if (y < image.Height)
				{
					bool masked = pixelColor.A == 255;
					if (transparentMeansMasked)
						masked = !masked;

					if (mask[x] == null)
						mask[x] = new bool[image.Height];
					mask[x][y] = masked;

					if (debugMask != null)
						debugMask.SetPixel(x, y, masked ? debugDrawColor.Value : Color.Transparent);
				}


				//Map the 1D array to (x,y)
				x++;
				if( x >= image.Width)
				{
					x=0;
					y++;
				}

			}

			//Save the duplicate
			if (saveMaskToFile != null)
				debugMask.Save (saveMaskToFile);

			AGSObject debugDraw = null;
			if (debugDrawColor != null)
			{
				debugDraw = new AGSObject (new AGSSprite ());
				debugDraw.Image = _factory.LoadImage(debugMask);
				debugDraw.Anchor = new AGSPoint ();
			}

			return new AGSMask (mask, debugDraw);
		}

		#endregion
	}
}

