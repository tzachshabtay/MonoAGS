using System;
using AGS.API;

namespace AGS.Engine
{
    public class GLTexture : ITexture
    {
        private ITextureConfig _config;
        private IGraphicsBackend _graphics;

        public GLTexture(ITextureConfig config, IGraphicsBackend graphics)
        {
            _graphics = graphics;
            if (Environment.CurrentManagedThreadId != AGSGame.UIThreadID)
            {
                throw new InvalidOperationException("Must generate textures on the UI thread");
            }
            ID = _graphics.GenTexture();
            Config = config ?? new AGSTextureConfig();
        }

        public ITextureConfig Config
        {
            get
            {
                return _config;
            }

            set
            {
                _config = value;
                applyConfig();
            }
        }

        public int ID { get; private set; }

        private void applyConfig()
        {
            _graphics.BindTexture2D(ID);
            _graphics.SetTextureMinFilter(_config.ScaleDownFilter);
            _graphics.SetTextureMagFilter(_config.ScaleUpFilter);
            _graphics.SetTextureWrapS(_config.WrapX);
            _graphics.SetTextureWrapT(_config.WrapY);
        }
    }
}
