using System;
using System.Diagnostics;
using System.IO;
using AGS.API;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace AGS.Engine
{
    public class ImageSharpBitmapLoader : IBitmapLoader
    {
        private readonly IGraphicsBackend _graphics;

        public ImageSharpBitmapLoader(IGraphicsBackend graphics)
        {
            _graphics = graphics;
        }

        public IBitmap Load(Stream stream)
        {
            var format = Image.DetectFormat(stream);
            var b = Image.Load(stream);
            if (format.Name == "BMP")
            {
                //workaround for this bug: https://github.com/SixLabors/ImageSharp/issues/581
                for (int x = 0; x < b.Width; x++)
                {
                    for (int y = 0; y < b.Height; y++)
                    {
                        var c = b[x, y];
                        b[x, y] = new Rgba32(c.R, c.G, c.B, 255);
                    }
                }
            }
            return new ImageSharpBitmap(b, _graphics);
        }

        public IBitmap Load(int width, int height)
        {
            return new ImageSharpBitmap(new Image<Rgba32>(width, height), _graphics);
        }
    }
}