using System;
using AGS.API;

namespace AGS.Engine
{
    [PropertyFolder]
    public class GLTexture : ITexture, IDisposable
    {
        private ITextureConfig _config;
        private IGraphicsBackend _graphics;
        private IRenderMessagePump _messagePump;

        public GLTexture(ITextureConfig config, IGraphicsBackend graphics, IRenderMessagePump messagePump)
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
            dispose(); 
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

        public override string ToString() => $"Texture ID: {ID}";

        public void Dispose()
        {
            dispose();
            GC.SuppressFinalize(this);
        }

        private void applyConfig()
        {
            if (Environment.CurrentManagedThreadId != AGSGame.UIThreadID)
            {
                _messagePump.Post(_ => applyConfigOnUiThread(), null);
            }
            else applyConfigOnUiThread();
        }

		private void applyConfigOnUiThread()
		{
			_graphics.BindTexture2D(ID);
			_graphics.SetTextureMinFilter(_config.ScaleDownFilter);
			_graphics.SetTextureMagFilter(_config.ScaleUpFilter);
			_graphics.SetTextureWrapS(_config.WrapX);
			_graphics.SetTextureWrapT(_config.WrapY);
		}

        private void dispose()
        {
            if (ID != 0) _messagePump.Post(_ => _graphics.DeleteTexture(ID), null);
        }
    }
}
