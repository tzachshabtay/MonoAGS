using System;
using AGS.API;

namespace AGS.Engine
{
    [PropertyFolder]
    public class GLImage : IImage, IDisposable
    {
        public GLImage()
        {
            Width = 1f;
            Height = 1f;
            ID = "";
            OnImageDisposed = new AGSEvent();
        }

        public GLImage(IBitmap bitmap, string id, ITexture texture, ISpriteSheet spriteSheet, ILoadImageConfig loadConfig)
        {
            OnImageDisposed = new AGSEvent();
            OriginalBitmap = bitmap;
            Width = bitmap.Width;
            Height = bitmap.Height;
            ID = id;
            Texture = texture;
            SpriteSheet = spriteSheet;
            LoadConfig = loadConfig;
        }

        ~GLImage()
        {
            dispose(false);
        }

        [Property(Browsable = false)]
        public IBitmap OriginalBitmap { get; private set; }

        public float Width { get; private set; }
        public float Height { get; private set; }

        public string ID { get; private set; }
        public ITexture Texture { get; private set; }

        public ISpriteSheet SpriteSheet { get; private set; }
        public ILoadImageConfig LoadConfig { get; private set; }

        public IBlockingEvent OnImageDisposed { get; private set; }

        public override string ToString()
        {
            return ID ?? base.ToString();
        }

        public void Dispose()
        {
            dispose(true);
            GC.SuppressFinalize(this);
        }

        private void dispose(bool disposing)
        {
            Texture = null;
            var onImageDisposed = OnImageDisposed;
            if (onImageDisposed != null) onImageDisposed.Invoke();
            OnImageDisposed = null;
        }
	}
}

