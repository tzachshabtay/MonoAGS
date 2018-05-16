using System;
using System.Collections.Generic;
using System.IO;
using AGS.API;
using SkiaSharp;

namespace AGS.Engine
{
    public class SkiaBitmap : IBitmap, IDisposable
    {
        private readonly SKBitmap _bitmap;
        private readonly IGraphicsBackend _graphics;

        public SkiaBitmap(SKBitmap bitmap, IGraphicsBackend graphics)
        {
            _bitmap = bitmap;
            _graphics = graphics;
            Width = bitmap.Width;
            Height = bitmap.Height;
        }

        ~SkiaBitmap()
        {
            dispose(false);
        }

        public int Width { get; }

        public int Height { get; }

        public IBitmap ApplyArea(IAreaComponent area)
        {
            //todo: performance can be improved by only creating a bitmap the size of the area, and not the entire background.
            //This will require to change the rendering as well to offset the location
            byte zero = (byte)0;
            SKBitmap output = new SKBitmap(Width, Height, SKImageInfo.PlatformColorType, SKAlphaType.Unpremul);
            using (FastSkiaBitmap inBmp = new FastSkiaBitmap(_bitmap))
            {
                using (FastSkiaBitmap outBmp = new FastSkiaBitmap(output, true))
                {
                    for (int y = 0; y < Height; y++)
                    {
                        int bitmapY = Height - y - 1;
                        for (int x = 0; x < Width; x++)
                        {
                            Color color = inBmp.GetPixel(x, bitmapY);
                            byte alpha = area.IsInArea(new PointF(x, y)) ? color.A : zero;
                            outBmp.SetPixel(x, bitmapY, color.WithAlpha(alpha));
                        }
                    }
                }
            }

            return new SkiaBitmap(output, _graphics);
        }

        public void Clear()
        {
            _bitmap.Erase(SKColors.White);
        }

        public IMask CreateMask(IGameFactory factory, string path, bool transparentMeansMasked = false, Color? debugDrawColor = null, string saveMaskToFile = null, string id = null)
        {
            SKBitmap debugMask = null;
            FastSkiaBitmap debugMaskFast = null;
            int bitmapWidth = Width;
            int bitmapHeight = Height;
            if (saveMaskToFile != null || debugDrawColor != null)
            {
                debugMask = new SKBitmap(bitmapWidth, bitmapHeight);
                debugMaskFast = new FastSkiaBitmap(debugMask, true);
            }

            bool[][] mask = new bool[bitmapWidth][];
            Color drawColor = debugDrawColor != null ? debugDrawColor.Value : Colors.Black;

            using (FastSkiaBitmap bitmapData = new FastSkiaBitmap(_bitmap))
            {
                for (int x = 0; x < bitmapWidth; x++)
                {
                    for (int y = 0; y < bitmapHeight; y++)
                    {
                        bool masked = bitmapData.IsOpaque(x, y);

                        if (transparentMeansMasked)
                            masked = !masked;

                        if (mask[x] == null)
                            mask[x] = new bool[bitmapHeight];
                        mask[x][bitmapHeight - y - 1] = masked;

                        debugMaskFast?.SetPixel(x, y, masked ? drawColor : Colors.Transparent);
                    }
                }
            }

            debugMaskFast?.Dispose();

            //Save the duplicate
            if (saveMaskToFile != null)
                saveToFile(debugMask, saveMaskToFile);

            IObject debugDraw = null;
            if (debugDrawColor != null)
            {
                debugDraw = factory.Object.GetAdventureObject(id ?? path ?? "Mask Drawable");
                debugDraw.Image = factory.Graphics.LoadImage(new SkiaBitmap(debugMask, _graphics), null, path);
                debugDraw.Pivot = new PointF();
            }

            return new AGSMask(mask, debugDraw);
        }

        public IBitmap Crop(Rectangle rectangle)
        {
            SKBitmap cropped = new SKBitmap(rectangle.Width, rectangle.Height);
            _bitmap.ExtractSubset(cropped, rectangle.Convert());
            return new SkiaBitmap(cropped, _graphics);
        }

        public Color GetPixel(int x, int y)
        {
            return _bitmap.GetPixel(x, y).Convert();
        }

        public IBitmapTextDraw GetTextDraw()
        {
            throw new NotImplementedException();
        }

        public void LoadTexture(int? textureToBind)
        {
            var scan0 = _bitmap.PeekPixels().GetPixels();

            if (textureToBind != null)
                _graphics.BindTexture2D(textureToBind.Value);

            _graphics.TexImage2D(Width, Height, scan0);
        }

        public void MakeTransparent(Color color)
        {
            using (FastSkiaBitmap bmp = new FastSkiaBitmap(_bitmap))
            {
                for (int x = 0; x < Width; x++)
                {
                    for (int y = 0; y < Height; y++)
                    {
                        if (bmp.GetPixel(x, y).Equals(color)) bmp.SetPixel(x, y, Colors.Transparent);
                    }
                }
            }
        }

        public void SaveToFile(string path)
        {
            saveToFile(_bitmap, path);
        }

        public void SetPixel(Color color, int x, int y)
        {
            _bitmap.SetPixel(x, y, color.Convert());
        }

        public void SetPixels(Color color, List<Point> points)
        {
            using (FastSkiaBitmap bmp = new FastSkiaBitmap(_bitmap))
            {
                foreach (var point in points)
                {
                    bmp.SetPixel(point.X, point.Y, color);
                }
            }
        }

        public void Dispose()
        {
            dispose(true);
            GC.SuppressFinalize(this);
        }

        private void dispose(bool disposing)
        {
            _bitmap?.Dispose();
        }

        private void saveToFile(SKBitmap bitmap, string path)
        {
            using (var image = SKImage.FromBitmap(bitmap))
            using (var data = image.Encode(SKEncodedImageFormat.Png, 100))
            {
                using (var stream = File.OpenWrite(path))
                {
                    data.SaveTo(stream);
                }
            }
        }
    }
}