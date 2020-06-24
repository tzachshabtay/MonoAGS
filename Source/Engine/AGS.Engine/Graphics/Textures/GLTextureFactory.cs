﻿using AGS.API;

namespace AGS.Engine
{
    public class GLTextureFactory : ITextureFactory
    {
        private static ITexture _emptyTexture;
        private readonly IGraphicsFactory _graphicsFactory;
        private readonly IBitmapLoader _bitmapLoader;

        public GLTextureFactory(IGraphicsFactory graphicsFactory, IBitmapLoader bitmapLoader)
        {
            _graphicsFactory = graphicsFactory;
            _bitmapLoader = bitmapLoader;
            _emptyTexture = _emptyTexture ?? initEmptyTexture();
        }

        public static ITexture EmptyTexture => _emptyTexture;

        public ITexture CreateTexture(string path)
        {
            if (string.IsNullOrEmpty(path)) return _emptyTexture;
            return _graphicsFactory.LoadImage(path).Texture;
        }

        private ITexture initEmptyTexture()
        {
            var bitmap = _bitmapLoader.Load(1, 1);
            bitmap.SetPixel(Colors.White, 0, 0);
            var config = new AGSLoadImageConfig(config: new AGSTextureConfig(scaleUp: ScaleUpFilters.Nearest));
            return _graphicsFactory.LoadImage(bitmap, config).Texture;
        }
    }
}