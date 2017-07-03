using System;
using AGS.API;

namespace AGS.Engine
{
    public class GLTexture : ITexture, IDisposable
    {
        private ITextureConfig _config;
        private IGraphicsBackend _graphics;
        private IMessagePump _messagePump;

        public GLTexture(ITextureConfig config, IGraphicsBackend graphics, IMessagePump messagePump)
        {
            _graphics = graphics;
            _messagePump = messagePump;
            if (Environment.CurrentManagedThreadId != AGSGame.UIThreadID)
            {
                throw new InvalidOperationException("Must generate textures on the UI thread");
            }
            ID = _graphics.GenTexture();
            Config = config ?? new AGSTextureConfig();
        }

        ~GLTexture()
        {
            dispose(false); 
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

        public void Dispose()
        {
            dispose(true);
            GC.SuppressFinalize(this);
        }

        private void applyConfig()
        {
            _graphics.BindTexture2D(ID);
            _graphics.SetTextureMinFilter(_config.ScaleDownFilter);
            _graphics.SetTextureMagFilter(_config.ScaleUpFilter);
            _graphics.SetTextureWrapS(_config.WrapX);
            _graphics.SetTextureWrapT(_config.WrapY);
        }

        private void dispose(bool disposing)
        {
            if (ID != 0) _messagePump.Post(_ => _graphics.DeleteTexture(ID), null);
        }
    }
}
