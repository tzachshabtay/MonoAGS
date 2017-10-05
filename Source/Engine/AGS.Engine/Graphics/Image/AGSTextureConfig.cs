using AGS.API;

namespace AGS.Engine
{
    [PropertyFolder]
    public class AGSTextureConfig : ITextureConfig
    {
        public AGSTextureConfig(ScaleDownFilters scaleDown = ScaleDownFilters.Nearest,
                                ScaleUpFilters scaleUp = ScaleUpFilters.Nearest,
                                TextureWrap wrapX = TextureWrap.Clamp,
                                TextureWrap wrapY = TextureWrap.Clamp)
        {
            ScaleDownFilter = scaleDown;
            ScaleUpFilter = scaleUp;
            WrapX = wrapX;
            WrapY = wrapY;
        }

        public ScaleDownFilters ScaleDownFilter { get; private set; }

        public ScaleUpFilters ScaleUpFilter { get; private set; }

        public TextureWrap WrapX { get; private set; }

        public TextureWrap WrapY { get; private set; }
    }
}
