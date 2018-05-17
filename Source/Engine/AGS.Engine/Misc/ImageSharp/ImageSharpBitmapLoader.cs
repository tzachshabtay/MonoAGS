using System;
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
            return new ImageSharpBitmap(Image.Load(stream), _graphics);
        }

        public IBitmap Load(int width, int height)
        {
            return new ImageSharpBitmap(new Image<Rgba32>(width, height), _graphics);
        }
    }
}