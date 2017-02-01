using System;
using System.Runtime.InteropServices;
using AGS.API;
using CoreGraphics;
using UIKit;

namespace AGS.Engine.IOS
{
    public class IOSBitmap : IBitmap
    {
        private UIImage _image;
        private readonly IGraphicsBackend _graphics;

        public IOSBitmap(UIImage image, IGraphicsBackend graphics)
        {
            _image = image;
            _graphics = graphics;
        }

        public int Height { get { return (int)_image.Size.Height; } }

        public int Width { get { return (int)_image.Size.Width; } }

        public IBitmap ApplyArea(IArea area)
        {
            //todo: performance can be improved by only creating a bitmap the size of the area, and not the entire background.
            //This will require to change the rendering as well to offset the location
            byte zero = (byte)0;
            var output = IOSBitmapLoader.Create(Width, Height);

            using (FastBitmap inBmp = new FastBitmap(_image.CGImage))
            {
                using (FastBitmap outBmp = new FastBitmap(output.CGImage, true))
                {
                    for (int y = 0; y < Height; y++)
                    {
                        int bitmapY = Height - y - 1;
                        for (int x = 0; x < Width; x++)
                        {
                            var color = inBmp.GetPixel(x, bitmapY);
                            byte alpha = area.IsInArea(new AGS.API.PointF(x, y)) ? color.A : zero;
                            outBmp.SetPixel(x, bitmapY, Color.FromRgba(color.R, color.G, color.B, alpha));
                        }
                    }
                }
            }

            return new IOSBitmap(output, _graphics);
        }

        public void Clear()
        {
            UIGraphics.BeginImageContext(new CGSize(Width, Height));
            var context = UIGraphics.GetCurrentContext();
            context.SaveState();
            CGRect rect = new CGRect(0f, 0f, Width, Height);
            context.DrawImage(rect, _image.CGImage);
            Color color = Colors.Transparent;
            context.SetFillColor(color.R / 255f, color.G / 255f, color.B / 255f, color.A / 255f);
            context.FillRect(new CGRect(0, 0, Width, Height));
            context.RestoreState();
            _image = UIGraphics.GetImageFromCurrentImageContext();
        }

        public IMask CreateMask(IGameFactory factory, string path, bool transparentMeansMasked = false, Color? debugDrawColor = default(Color?), string saveMaskToFile = null, string id = null)
        {
            UIImage debugMask = null;
            FastBitmap debugMaskFast = null;
            if (saveMaskToFile != null || debugDrawColor != null)
            {
                debugMask = IOSBitmapLoader.Create(Width, Height);
                debugMaskFast = new FastBitmap(debugMask.CGImage, true);
            }

            bool[][] mask = new bool[Width][];
            Color drawColor = debugDrawColor != null ? debugDrawColor.Value : Colors.Black;

            using (FastBitmap bitmapData = new FastBitmap(_image.CGImage))
            {
                for (int x = 0; x < Width; x++)
                {
                    for (int y = 0; y < Height; y++)
                    {
                        var pixelColor = bitmapData.GetPixel(x, y);

                        bool masked = pixelColor.A == 255;
                        if (transparentMeansMasked)
                            masked = !masked;

                        if (mask[x] == null)
                            mask[x] = new bool[Height];
                        mask[x][Height - y - 1] = masked;

                        if (debugMask != null)
                        {
                            debugMaskFast.SetPixel(x, y, masked ? drawColor : global::Android.Graphics.Color.Transparent);
                        }
                    }
                }
            }

            if (debugMask != null)
                debugMaskFast.Dispose();

            //Save the duplicate
            if (saveMaskToFile != null)
            {
                debugMask.AsPNG().Save(saveMaskToFile, true);
            }

            IObject debugDraw = null;
            if (debugDrawColor != null)
            {
                debugDraw = factory.Object.GetObject(id ?? path ?? "Mask Drawable");
                debugDraw.Image = factory.Graphics.LoadImage(new IOSBitmap(debugMask, _graphics), null, path);
                debugDraw.Anchor = new AGS.API.PointF();
            }

            return new AGSMask(mask, debugDraw);
        }

        public IBitmap Crop(Rectangle rectangle)
        {
            var cropped = _image.CGImage.WithImageInRect(new CGRect(rectangle.X, rectangle.Y, rectangle.Width, rectangle.Height));
            UIImage image = new UIImage(cropped);
            return new IOSBitmap(image, _graphics);
        }

        //http://stackoverflow.com/questions/3284185/get-pixel-color-of-uiimage
        public Color GetPixel(int x, int y)
        {
            var pixelData = _image.CGImage.DataProvider.CopyData();
            var data = pixelData.Bytes;
            int pixelIndex = ((Width * y) + x) * 4; // 4 bytes per pixel

            byte[] rgba = new byte[4];
            Marshal.Copy(data, rgba, pixelIndex, rgba.Length);

            return Color.FromRgba(rgba[0], rgba[1], rgba[2], rgba[3]);
        }

        public IBitmapTextDraw GetTextDraw()
        {
            return new IOSBitmapTextDraw(_image);
        }

        //https://github.com/xamarin/mobile-samples/blob/master/TexturedCubeES30/TexturedCubeiOS/EAGLView.cs
        public void LoadTexture(int? textureToBind)
        {
            if (textureToBind != null)
                _graphics.BindTexture2D(textureToBind.Value);

            nint width = _image.CGImage.Width;
            nint height = _image.CGImage.Height;

            CGColorSpace colorSpace = CGColorSpace.CreateDeviceRGB();
            byte[] imageData = new byte[height * width * 4];
            CGContext context = new CGBitmapContext(imageData, width, height, 8, 4 * width, colorSpace,
                                    CGBitmapFlags.PremultipliedLast | CGBitmapFlags.ByteOrder32Big);

            context.TranslateCTM(0, height);
            context.ScaleCTM(1, -1);
            colorSpace.Dispose();
            context.ClearRect(new CGRect(0, 0, width, height));
            context.DrawImage(new CGRect(0, 0, width, height), _image.CGImage);

            unsafe
            {
                fixed (byte* p = imageData)
                {
                    IntPtr ptr = (IntPtr)p;
                    _graphics.TexImage2D(Width, Height, p);
                }
            }

            context.Dispose();
        }

        //http://stackoverflow.com/questions/633722/how-to-make-one-color-transparent-on-a-uiimage
        public void MakeTransparent(Color color)
        {
            var transparent = Colors.Transparent;
            /*var colorSpace = CGColorSpace.CreateDeviceRGB();
            const int bytesPerPixel = 4;
            int bytesPerRow = bytesPerPixel * Width;
            const int bitsPerComponent = 8;
            int bitmapBytesCount = bytesPerRow * Height;

            byte[] data = new byte[bitmapBytesCount];
            var context = new CGBitmapContext(data, Width, Height, bitsPerComponent, bytesPerRow,
                                              colorSpace, CGBitmapFlags.PremultipliedLast | CGBitmapFlags.ByteOrder32Big);
            colorSpace.Dispose();
            context.DrawImage(new CGRect(0, 0, Width, Height), _image.CGImage);

            for (int byteIndex = 0; byteIndex < bitmapBytesCount; byteIndex += bytesPerPixel)
            {
                if (data[byteIndex] == color.R
                    && data[byteIndex + 1] == color.G
                    && data[byteIndex + 2] == color.B
                    && data[byteIndex + 3] == color.A)
                {
                    data[byteIndex] = transparent.R;
                    data[byteIndex + 1] = transparent.G;
                    data[byteIndex + 2] = transparent.B;
                    data[byteIndex + 3] = transparent.A;
                }
            }
            var image = context.ToImage();
            _image = new UIImage(image);
            image.Dispose();
            context.Dispose();*/

            using (FastBitmap fastBitmap = new FastBitmap(_image.CGImage))
            {
                for (int x = 0; x < Width; x++)
                {
                    for (int y = 0; y < Height; y++)
                    {
                        if (fastBitmap.GetPixel(x, y) == c)
                            fastBitmap.SetPixel(x, y, transparent);
                    }
                }
            }
        }

        public void SaveToFile(string path)
        {
            _image.AsPNG().Save(path, true);
        }

        //http://stackoverflow.com/questions/16462129/iphone-how-to-change-color-of-particular-pixel-of-a-uiimage
        public void SetPixel(Color color, int x, int y)
        {
            UIGraphics.BeginImageContext(new CGSize(Width, Height));
            var context = UIGraphics.GetCurrentContext();
            context.SaveState();
            CGRect rect = new CGRect(0f, 0f, Width, Height);
            context.DrawImage(rect, _image.CGImage);
            context.SetFillColor(color.R / 255f, color.G / 255f, color.B / 255f, color.A / 255f);
            context.FillRect(new CGRect(x, y, 1, 1));
            context.RestoreState();
            _image = UIGraphics.GetImageFromCurrentImageContext();
        }
    }
}
