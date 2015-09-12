using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;

namespace Engine
{
	public static class GraphicsUtils
	{
		public static bool[][] LoadMask(string path, out Bitmap debugMask, 
			bool colorIsTrue = true, Color? maskColor = null, string debugMaskPath = null)
		{
			Color color = maskColor == null ? Color.FromArgb(0,0,0,0) : maskColor.Value;
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
		    debugMask = null;
			if (debugMaskPath != null)
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
					bool masked = color.R == pixelColor.R && 
						color.G == pixelColor.G &&
					              color.B == pixelColor.B && color.A == pixelColor.A;
					if (!colorIsTrue)
						masked = !masked;

					if (mask [x] == null)
						mask [x] = new bool[image.Height];
					mask [x][y] = masked;

					if (debugMask != null)
						debugMask.SetPixel(x,y,masked ? Color.White : Color.Black);

					if (!masked) 
					{
					}
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
			if (debugMask != null)
				debugMask.Save (debugMaskPath);

			return mask;
		}
	}
}

