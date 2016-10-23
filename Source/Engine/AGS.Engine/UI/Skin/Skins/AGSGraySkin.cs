using AGS.API;

namespace AGS.Engine
{
    public class AGSGraySkin
    {
        private AGSColoredSkin _skin;

        public AGSGraySkin(IGraphicsFactory factory, IGLUtils glUtils)
        {
            _skin = new AGSColoredSkin(factory)
            {
                ButtonIdleBackColor = Colors.DimGray,
                ButtonHoverBackColor = Colors.LightGray,
                ButtonPushedBackColor = Colors.LightYellow,
                ButtonBorderStyle = AGSBorders.SolidColor(glUtils, Colors.Black, 1f),
                TextBoxBackColor = Colors.DimGray,
                TextBoxBorderStyle = AGSBorders.SolidColor(glUtils, Colors.Black, 1f),
                CheckboxCheckedColor = Colors.SlateGray,
                CheckboxNotCheckedColor = Colors.DimGray,
                CheckboxHoverCheckedColor = Colors.LightGray,
                CheckboxHoverNotCheckedColor = Colors.LightGray,
                CheckboxBorderStyle = AGSBorders.SolidColor(glUtils, Colors.Black, 1f),
                DialogBoxColor = Colors.DarkGray,
                DialogBoxBorder = AGSBorders.SolidColor(glUtils, Colors.Black, 2f)
            };
        }

        public ISkin CreateSkin()
        {
            return _skin.CreateSkin();
        }
    }
}
