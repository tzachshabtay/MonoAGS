using System;
using System.IO;

namespace AGS.API
{
	public interface IBitmapLoader
	{
		IBitmap Load(Stream stream);
		IBitmap Load(string path);
		IBitmap Load(int width, int height);
	}
}

