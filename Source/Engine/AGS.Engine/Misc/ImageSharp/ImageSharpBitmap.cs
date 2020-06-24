using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using AGS.API;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Advanced;
using SixLabors.ImageSharp.Drawing.Processing;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;

namespace AGS.Engine
{
    public class ImageSharpBitmap : IBitmap
    {
        private Image<Rgba32> _image;
        private readonly IGraphicsBackend _graphics;
        private static SolidBrush _whiteBrush = Brushes.Solid(SixLabors.ImageSharp.Color.White);

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

        public IMask CreateMask(IGameFactory factory, string path, bool transparentMeansMasked = false, AGS.API.Color? debugDrawColor = null, string saveMaskToFile = null, string id = null)
        {
            Image<Rgba32> debugMask = null;
            int bitmapWidth = Width;
            int bitmapHeight = Height;
            if (saveMaskToFile != null || debugDrawColor != null)
            {
                debugMask = new Image<Rgba32>(bitmapWidth, bitmapHeight);
            }

            bool[][] mask = new bool[bitmapWidth][];
            Rgba32 drawColor = (debugDrawColor != null ? debugDrawColor.Value.Convert() : SixLabors.ImageSharp.Color.Black).ToPixel<Rgba32>();

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

                    debugMask[x, y] = masked ? drawColor : SixLabors.ImageSharp.Color.Transparent.ToPixel<Rgba32>();
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

        public IBitmap Crop(AGS.API.Rectangle rectangle)
        {
            var clone = _image.Clone(x => x.Crop(new SixLabors.ImageSharp.Rectangle(rectangle.X, rectangle.Y, rectangle.Width, rectangle.Height)));
            return new ImageSharpBitmap(clone, _graphics);
        }

        public AGS.API.Color GetPixel(int x, int y)
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
            fixed (void* pin = &MemoryMarshal.GetReference(_image.GetPixelRowSpan(0)))
            {
                if (textureToBind != null)
                    _graphics.BindTexture2D(textureToBind.Value);

                _graphics.TexImage2D(Width, Height, (IntPtr)pin);
            }
        }

        public void MakeTransparent(AGS.API.Color color)
        {
            for (int x = 0; x < Width; x++)
            {
                for (int y = 0; y < Height; y++)
                {
                    if (_image[x, y].PackedValue == color.Value)
                    {
                        _image[x, y] = SixLabors.ImageSharp.Color.Transparent.ToPixel<Rgba32>();
                    }
                }
            }
        }

        public void SaveToFile(string path)
        {
            _image.Save(path);
        }

        public void SetPixel(AGS.API.Color color, int x, int y)
        {
            _image[x, y] = color.Convert();
        }

        public void SetPixels(AGS.API.Color color, List<AGS.API.Point> points)
        {
            var rgba = color.Convert();
            foreach (var point in points)
            {
                _image[point.X, point.Y] = rgba;
            }
        }
    }
}