using System;
using AGS.API;
using OpenTK.Graphics.OpenGL;

namespace AGS.Engine
{
    public class GLTexture : ITexture
    {
        private ITextureConfig _config;

        public GLTexture(ITextureConfig config)
        {
            if (Environment.CurrentManagedThreadId != AGSGame.UIThreadID)
            {
                throw new InvalidOperationException("Must generate textures on the UI thread");
            }
            int tex;
            GL.Hint(HintTarget.PerspectiveCorrectionHint, HintMode.Nicest);

            GL.GenTextures(1, out tex);
            ID = tex;
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
            GL.BindTexture(TextureTarget.Texture2D, ID);

            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, getMinFilter());
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, getMaxFilter());
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, getWrapMode(_config.WrapX));
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, getWrapMode(_config.WrapY));
        }

        private int getWrapMode(TextureWrap wrap)
        {
            switch (wrap)
            {
                case TextureWrap.Clamp: return (int)TextureWrapMode.ClampToEdge;
                case TextureWrap.MirroredRepeat: return (int)TextureWrapMode.MirroredRepeat;
                case TextureWrap.Repeat: return (int)TextureWrapMode.Repeat;
                default: throw new NotSupportedException(wrap.ToString());
            }
        }

        private int getMinFilter()
        {
            switch (_config.ScaleDownFilter)
            {
                case ScaleDownFilters.Nearest: return (int)TextureMinFilter.Nearest;
                case ScaleDownFilters.NearestMipmapLinear: return (int)TextureMinFilter.NearestMipmapLinear;
                case ScaleDownFilters.NearestMipmapNearest: return (int)TextureMinFilter.NearestMipmapNearest;
                case ScaleDownFilters.Linear: return (int)TextureMinFilter.Linear;
                case ScaleDownFilters.LinearMipmapLinear: return (int)TextureMinFilter.LinearMipmapLinear;
                case ScaleDownFilters.LinearMipmapNearest: return (int)TextureMinFilter.LinearMipmapNearest;
                default: throw new NotSupportedException(_config.ScaleDownFilter.ToString());
            }
        }

        private int getMaxFilter()
        {
            switch (_config.ScaleUpFilter)
            {
                case ScaleUpFilters.Linear: return (int)TextureMagFilter.Linear;
                case ScaleUpFilters.Nearest: return (int)TextureMagFilter.Nearest;
                default: throw new NotSupportedException(_config.ScaleUpFilter.ToString());
            }
        }
    }
}
