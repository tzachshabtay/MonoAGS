using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using AGS.API;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Advanced;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp.Processing.Drawing.Brushes;
using SixLabors.ImageSharp.Processing.Transforms;

namespace AGS.Engine
{
    public class ImageSharpBitmap : IBitmap
    {
        private Image<Rgba32> _image;
        private readonly IGraphicsBackend _graphics;
        private static SolidBrush<Rgba32> _whiteBrush = Brushes.Solid(Rgba32.White);

        public ImageSharpBitmap(Image<Rgba32> image, IGraphicsBackend graphics)
        {
            _image = image;
            Width = image.Width;
            Height = image.Height;
            _graphics = graphics;
        }

        public int Width { get; }

        public int Height { get; }

        public IBitmap ApplyArea(IAreaComponent area)
        {
            //todo: performance can be improved by only creating a bitmap the size of the area, and not the entire background.
            //This will require to change the rendering as well to offset the location
            byte zero = (byte)0;
            var output = new Image<Rgba32>(Width, Height);

            for (int y = 0; y < Height; y++)
            {
                int bitmapY = Height - y - 1;
                for (int x = 0; x < Width; x++)
                {
                    var color = _image[x, bitmapY];
                    byte alpha = area.IsInArea(new AGS.API.PointF(x, y)) ? color.A : zero;
                    output[x, bitmapY] = new Rgba32(color.R, color.G, color.B, alpha);
                }
            }

            return new ImageSharpBitmap(output, _graphics);
        }

        public void Clear()
        {
            _image.Clear(_whiteBrush);
        }

        public IMask CreateMask(IGameFactory factory, string path, bool transparentMeansMasked = false, Color? debugDrawColor = null, string saveMaskToFile = null, string id = null)
        {
            Image<Rgba32> debugMask = null;
            int bitmapWidth = Width;
            int bitmapHeight = Height;
            if (saveMaskToFile != null || debugDrawColor != null)
            {
                debugMask = new Image<Rgba32>(bitmapWidth, bitmapHeight);
            }

            bool[][] mask = new bool[bitmapWidth][];
            Rgba32 drawColor = debugDrawColor != null ? debugDrawColor.Value.Convert() : Rgba32.Black;

            for (int x = 0; x < bitmapWidth; x++)
            {
                for (int y = 0; y < bitmapHeight; y++)
                {
                    bool masked = _image[x, y].A == 255;

                    if (transparentMeansMasked)
                        masked = !masked;

                    if (mask[x] == null)
                        mask[x] = new bool[bitmapHeight];
                    mask[x][bitmapHeight - y - 1] = masked;

                    debugMask[x, y] = masked ? drawColor : Rgba32.Transparent;
                }
            }

            //Save the duplicate
            if (saveMaskToFile != null)
                debugMask.Save(saveMaskToFile);

            IObject debugDraw = null;
            if (debugDrawColor != null)
            {
                debugDraw = factory.Object.GetAdventureObject(id ?? path ?? "Mask Drawable");
                debugDraw.Image = factory.Graphics.LoadImage(new ImageSharpBitmap(debugMask, _graphics), null, path);
                debugDraw.Pivot = new AGS.API.PointF();
            }

            return new AGSMask(mask, debugDraw);
        }

        public IBitmap Crop(Rectangle rectangle)
        {
            var clone = _image.Clone(x => x.Crop(new SixLabors.Primitives.Rectangle(rectangle.X, rectangle.Y, rectangle.Width, rectangle.Height)));
            return new ImageSharpBitmap(clone, _graphics);
        }

        public Color GetPixel(int x, int y)
        {
            var rgba = _image[x, y];
            return rgba.Convert();
        }

        public IBitmapTextDraw GetTextDraw()
        {
            return new ImageSharpTextDraw(_image);
        }

        public unsafe void LoadTexture(int? textureToBind)
        {
            fixed (void* pin = &_image.DangerousGetPinnableReferenceToPixelBuffer())
            {
                if (textureToBind != null)
                    _graphics.BindTexture2D(textureToBind.Value);

                _graphics.TexImage2D(Width, Height, (IntPtr)pin);
            }
        }

        public void MakeTransparent(Color color)
        {
            for (int x = 0; x < Width; x++)
            {
                for (int y = 0; y < Height; y++)
                {
                    if (_image[x, y].PackedValue == color.Value)
                    {
                        _image[x, y] = Rgba32.Transparent;
                    }
                }
            }
        }

        public void SaveToFile(string path)
        {
            _image.Save(path);
        }

        public void SetPixel(Color color, int x, int y)
        {
            _image[x, y] = color.Convert();
        }

        public void SetPixels(Color color, List<Point> points)
        {
            var rgba = color.Convert();
            foreach (var point in points)
            {
                _image[point.X, point.Y] = rgba;
            }
        }
    }
}