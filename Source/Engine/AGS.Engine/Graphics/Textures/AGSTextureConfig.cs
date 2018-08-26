using AGS.API;

namespace AGS.Engine
{
    [PropertyFolder]
    [ConcreteImplementation(DisplayName = "Texture Config")]
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

        public override string ToString() => $"{scaleToString()}, {wrapToString()}";

        private string scaleToString()
        {
            if (ScaleDownFilter == ScaleDownFilters.Nearest && ScaleUpFilter == ScaleUpFilters.Nearest)
                return "Nearest";
            if (ScaleDownFilter == ScaleDownFilters.Nearest && ScaleUpFilter == ScaleUpFilters.Linear)
                return "Linear";
            return $"{ScaleDownFilter}, {ScaleUpFilter}";
        }

        private string wrapToString()
        {
            if (WrapX == WrapY) return WrapX.ToString();
            return $"{WrapX},{WrapY}";
        }
    }
}