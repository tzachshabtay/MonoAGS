using System;
using AGS.API;
using Android.Graphics;

namespace AGS.Engine.Android
{
	public class AndroidBitmapLoader : IBitmapLoader
	{
		public AndroidBitmapLoader()
		{
		}

		#region IBitmapLoader implementation

		public IBitmap Load(System.IO.Stream stream)
		{
			Bitmap bitmap = BitmapFactory.DecodeStream(stream);
			return new AndroidBitmap (bitmap);
		}

		public IBitmap Load(string path)
		{
			Bitmap bitmap = BitmapFactory.DecodeFile(path);
			return new AndroidBitmap (bitmap);
		}

		public IBitmap Load(int width, int height)
		{
			Bitmap bitmap = Bitmap.CreateBitmap(width, height, Bitmap.Config.Argb8888);
			return new AndroidBitmap (bitmap);
		}

		#endregion
	}
}

