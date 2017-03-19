extern alias IOS;

using System;
using System.Diagnostics;
using AGS.API;
using IOS::CoreGraphics;
using IOS::UIKit;

namespace AGS.Engine.IOS
{
    public class IOSBitmap : IBitmap
    {
        private UIImage _uiImage;
        private CGImage _cgImage;
        private readonly IGraphicsBackend _graphics;

        public IOSBitmap(UIImage image, IGraphicsBackend graphics)
        {
            setImage(image);
            _graphics = graphics;
        }

        public int Height { get; private set; }

        public int Width { get; private set; }

        private void setImage(UIImage image)
        {
            var currentImage = _uiImage;
            if (currentImage == image) return;
            _uiImage = image;
            _cgImage = image.CGImage;
            var size = image.Size;
            Height = (int)_uiImage.Size.Height;
            Width = (int)_uiImage.Size.Width;
            if (currentImage != null) currentImage.Dispose();
        }

        public IBitmap ApplyArea(IArea area)
        {
            //todo: performance can be improved by only creating a bitmap the size of the area, and not the entire background.
            //This will require to change the rendering as well to offset the location
            byte zero = (byte)0;
            UIImage output;

            using (FastBitmap inBmp = new FastBitmap(_cgImage))
            {
                using (FastBitmap outBmp = new FastBitmap(Width, Height))
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
                    output = outBmp.GetImage();
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
            context.DrawImage(rect, _cgImage);
            Color color = Colors.Transparent;
            context.SetFillColor(color.R / 255f, color.G / 255f, color.B / 255f, color.A / 255f);
            context.FillRect(new CGRect(0, 0, Width, Height));
            context.RestoreState();
            setImage(UIGraphics.GetImageFromCurrentImageContext());
        }

        public IMask CreateMask(IGameFactory factory, string path, bool transparentMeansMasked = false, Color? debugDrawColor = default(Color?), string saveMaskToFile = null, string id = null)
        {
            FastBitmap debugMaskFast = null;
            if (saveMaskToFile != null || debugDrawColor != null) 
            {
                debugMaskFast = new FastBitmap(Width, Height);
            }

            bool[][] mask = new bool[Width][];
            Color drawColor = debugDrawColor != null ? debugDrawColor.Value : Colors.Black;

            using (FastBitmap bitmapData = new FastBitmap(_cgImage))
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

                        if (debugMaskFast != null)
                        {
                            debugMaskFast.SetPixel(x, y, masked ? drawColor : Colors.Transparent);
                        }
                    }
                }
            }

            UIImage debugMask = null;
            if (debugMaskFast != null)
            {
                debugMask = debugMaskFast.GetImage();
                debugMaskFast.Dispose();
            }

            //Save the duplicate
            if (saveMaskToFile != null)
            {
                saveToFile(debugMask, saveMaskToFile);
            }

            IObject debugDraw = null;
            if (debugDrawColor != null)
            {
                debugDraw = factory.Object.GetObject(id ?? path ?? "Mask Drawable");
                debugDraw.Image = factory.Graphics.LoadImage(new IOSBitmap(debugMask, _graphics), null, path);
                debugDraw.Anchor = new AGS.API.PointF();
            }
            else if (debugMask != null) debugMask.Dispose();

            return new AGSMask(mask, debugDraw);
        }

        public IBitmap Crop(Rectangle rectangle)
        {
            var cropped = _cgImage.WithImageInRect(new CGRect(rectangle.X, rectangle.Y, rectangle.Width, rectangle.Height));
            UIImage image = new UIImage(cropped);
            return new IOSBitmap(image, _graphics);
        }

        public Color GetPixel(int x, int y)
        {
            using (FastBitmap bitmap = new FastBitmap(_cgImage))
            {
                return bitmap.GetPixel(x, y);
            }
        }

        public IBitmapTextDraw GetTextDraw()
        {
            return new IOSBitmapTextDraw(_uiImage);
        }

        //https://github.com/xamarin/mobile-samples/blob/master/TexturedCubeES30/TexturedCubeiOS/EAGLView.cs
        public void LoadTexture(int? textureToBind)
        {
            if (textureToBind != null)
                _graphics.BindTexture2D(textureToBind.Value);

            var width = _cgImage.Width;
            var height = _cgImage.Height;

            using (CGColorSpace colorSpace = CGColorSpace.CreateDeviceRGB())
            {
                byte[] imageData = new byte[height * width * 4];
                CGContext context = new CGBitmapContext(imageData, width, height, 8, 4 * width, colorSpace,
                                        CGBitmapFlags.PremultipliedLast | CGBitmapFlags.ByteOrder32Big);

                using (context)
                {
                    context.TranslateCTM(0, height);
                    context.ScaleCTM(1, -1);
                    context.ClearRect(new CGRect(0, 0, width, height));
                    context.DrawImage(new CGRect(0, 0, width, height), _cgImage);

                    unsafe
                    {
                        fixed (byte* p = imageData)
                        {
                            IntPtr ptr = (IntPtr)p;
                            _graphics.TexImage2D(Width, Height, ptr);
                        }
                    }
                }
            }
        }

        //http://stackoverflow.com/questions/633722/how-to-make-one-color-transparent-on-a-uiimage
        public void MakeTransparent(Color color)
        {
            var transparent = Colors.Transparent;
            using (FastBitmap fastBitmap = new FastBitmap(_cgImage))
            {
                for (int x = 0; x < Width; x++)
                {
                    for (int y = 0; y < Height; y++)
                    {
                        if (fastBitmap.GetPixel(x, y).Equals(color))
                            fastBitmap.SetPixel(x, y, transparent);
                    }
                }
                setImage(fastBitmap.GetImage());
            }
        }

        public void SaveToFile(string path)
        {
            saveToFile(_uiImage, path);
        }

        //http://stackoverflow.com/questions/16462129/iphone-how-to-change-color-of-particular-pixel-of-a-uiimage
        public void SetPixel(Color color, int x, int y)
        {
            UIGraphics.BeginImageContext(new CGSize(Width, Height));
            var context = UIGraphics.GetCurrentContext();
            context.SaveState();
            CGRect rect = new CGRect(0f, 0f, Width, Height);
            context.DrawImage(rect, _cgImage);
            context.SetFillColor(color.R / 255f, color.G / 255f, color.B / 255f, color.A / 255f);
            context.FillRect(new CGRect(x, y, 1, 1));
            context.RestoreState();
            setImage(UIGraphics.GetImageFromCurrentImageContext());
        }

        private void saveToFile(UIImage image, string path)
        {
            using (var imageData = image.AsPNG())
            {
                if (imageData == null)
                {
                    Debug.WriteLine("Saving image to path " + path + " failed");
                    return;
                }
                imageData.Save(path, true);
            }
        }
    }
}
