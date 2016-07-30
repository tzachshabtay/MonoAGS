using AGS.API;

namespace AGS.Engine
{
    public class AGSGraySkin
    {
        private AGSColoredSkin _skin;

        public AGSGraySkin(IGraphicsFactory factory)
        {
            _skin = new AGSColoredSkin(factory)
            {
                ButtonIdleBackColor = Colors.DimGray,
                ButtonHoverBackColor = Colors.LightGray,
                ButtonPushedBackColor = Colors.LightYellow,
                ButtonBorderStyle = AGSBorders.SolidColor(Colors.Black, 1f)
            };
        }

        public ISkin CreateSkin()
        {
            return _skin.CreateSkin();
        }
    }
}
