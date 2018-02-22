using System;
using AGS.API;

namespace AGS.Engine
{
    public class GLTextureFactory : ITextureFactory
    {
        private static Lazy<ITexture> _emptyTexture;
        private readonly IGraphicsFactory _graphicsFactory;
        private readonly IBitmapLoader _bitmapLoader;

        public GLTextureFactory(IGraphicsFactory graphicsFactory, IBitmapLoader bitmapLoader)
        {
            _graphicsFactory = graphicsFactory;
            _bitmapLoader = bitmapLoader;
            _emptyTexture = new Lazy<ITexture>(() => initEmptyTexture());
        }

        public static ITexture EmptyTexture => _emptyTexture.Value;

        public ITexture CreateTexture(string path)
        {
            if (string.IsNullOrEmpty(path)) return _emptyTexture.Value;
            return _graphicsFactory.LoadImage(path).Texture;
        }

        private ITexture initEmptyTexture()
        {
            var bitmap = _bitmapLoader.Load(1, 1);
            bitmap.SetPixel(Colors.White, 0, 0);
            return _graphicsFactory.LoadImage(bitmap, new AGSLoadImageConfig(config: new AGSTextureConfig(scaleUp: ScaleUpFilters.Nearest))).Texture;
        }
    }
}
