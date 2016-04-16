using System;
using AGS.API;
using System.IO;
using System.Drawing;

namespace AGS.Engine.Desktop
{
	public class DesktopBitmapLoader : IBitmapLoader
	{
		#region IBitmapLoader implementation

		public IBitmap Load(Stream stream)
		{
			return new DesktopBitmap (new Bitmap (stream));
		}

		public IBitmap Load(string path)
		{
			return new DesktopBitmap(new Bitmap (Image.FromFile(path)));
		}

		public IBitmap Load(int width, int height)
		{
			return new DesktopBitmap (new Bitmap (width, height));
		}

		#endregion
	}
}

