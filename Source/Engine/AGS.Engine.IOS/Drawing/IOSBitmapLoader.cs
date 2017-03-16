extern alias IOS;

using System.IO;
using AGS.API;
using IOS::CoreGraphics;
using IOS::Foundation;
using IOS::UIKit;

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
            using (NSData data = NSData.FromArray(buffer))
            {
                UIImage image = UIImage.LoadFromData(data);

                //For some reason, flipping the image vertically is required for IOS, see Alexey Podlasov's answer here: http://stackoverflow.com/questions/5404706/how-to-flip-uiimage-horizontally
                //todo: maybe instead of flipping the image at run-time we can flip it at compile-time for ios
                image = flipVertically(image);

                return new IOSBitmap(image, _graphics);
            }
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

        private UIImage flipVertically(UIImage image)
        { 
            UIGraphics.BeginImageContext(image.Size);
            var context = UIGraphics.GetCurrentContext();
            context.DrawImage(new CGRect(0f, 0f, image.Size.Width, image.Size.Height), image.CGImage);
            var tmpImage = image;
            image = UIGraphics.GetImageFromCurrentImageContext();
            tmpImage.Dispose();
            return image;
        }
    }
}
