using System.IO;
using AGS.API;
using CoreGraphics;
using Foundation;
using UIKit;

namespace AGS.Engine.IOS
{
    public class IOSBitmapLoader : IBitmapLoader
    {
        private readonly IGraphicsBackend _graphics;

        public IOSBitmapLoader(IGraphicsBackend graphics)
        {
            _graphics = graphics;
        }

        public IBitmap Load(Stream stream)
        {
            byte[] buffer = new byte[stream.Length];
            stream.Read(buffer, 0, buffer.Length);
            NSData data = NSData.FromArray(buffer);
            UIImage image = UIImage.LoadFromData(data);
            return new IOSBitmap(image, _graphics);
        }

        public IBitmap Load(int width, int height)
        {
            var image = Create(width, height);
            return new IOSBitmap(image, _graphics);
        }

        public static UIImage Create(int width, int height)
        { 
            UIGraphics.BeginImageContext(new CGSize(width, height));
            UIImage image = UIGraphics.GetImageFromCurrentImageContext();
            UIGraphics.EndImageContext();
            return image;
        }
    }
}
