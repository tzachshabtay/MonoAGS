using System;
using System.IO;
using AGS.API;
using SkiaSharp;

namespace AGS.Engine
{
    public class SkiaBitmapLoader : IBitmapLoader
    {
        private readonly IGraphicsBackend _graphics;

        public SkiaBitmapLoader(IGraphicsBackend graphics) => _graphics = graphics;

        public IBitmap Load(Stream stream)
        {
            var bitmap = SKBitmap.Decode(stream);
            return new SkiaBitmap(bitmap, _graphics);
        }

        public IBitmap Load(int width, int height)
        {
            var bitmap = new SKBitmap(width, height);
            return new SkiaBitmap(bitmap, _graphics);
        }
    }
}