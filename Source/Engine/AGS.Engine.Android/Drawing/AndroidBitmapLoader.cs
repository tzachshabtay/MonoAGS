using System;
using AGS.API;
using Android.Graphics;

namespace AGS.Engine.Android
{
	public class AndroidBitmapLoader : IBitmapLoader
	{
        private readonly IGraphicsBackend _graphics;

		public AndroidBitmapLoader(IGraphicsBackend graphics)
        {
            _graphics = graphics;
        }

		#region IBitmapLoader implementation

		public IBitmap Load(System.IO.Stream stream)
		{
			Bitmap bitmap = BitmapFactory.DecodeStream(stream);
            return new AndroidBitmap (bitmap, _graphics);
		}

		public IBitmap Load(int width, int height)
		{
			Bitmap bitmap = Bitmap.CreateBitmap(width, height, Bitmap.Config.Argb8888);
            return new AndroidBitmap (bitmap, _graphics);
		}

		#endregion
	}
}

