using AGS.API;
using System.IO;
using System.Drawing;

namespace AGS.Engine.Desktop
{
	public class DesktopBitmapLoader : IBitmapLoader
	{
        private readonly IGraphicsBackend _graphics;

        public DesktopBitmapLoader(IGraphicsBackend graphics)
        {
            _graphics = graphics;
        }

		#region IBitmapLoader implementation

		public IBitmap Load(Stream stream)
		{
            return new DesktopBitmap (new Bitmap (stream), _graphics);
		}

		public IBitmap Load(int width, int height)
		{
            return new DesktopBitmap (new Bitmap (width, height), _graphics);
		}

		#endregion
	}
}

