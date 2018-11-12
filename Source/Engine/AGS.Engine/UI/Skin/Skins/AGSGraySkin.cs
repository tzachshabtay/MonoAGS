using AGS.API;

namespace AGS.Engine
{
    public class AGSGraySkin
    {
        private readonly AGSColoredSkin _skin;

        public AGSGraySkin(IGLUtils glUtils, IRuntimeSettings settings)
        {
            var buttonBorder = AGSBorders.SolidColor(glUtils, settings, Colors.Black, 1f);
            _skin = new AGSColoredSkin
            {
                ButtonIdleAnimation = new ButtonAnimation(buttonBorder, null, Colors.DimGray),
                ButtonHoverAnimation = new ButtonAnimation(buttonBorder, null, Colors.LightGray),
                ButtonPushedAnimation = new ButtonAnimation(buttonBorder, null, Colors.LightYellow),
                TextBoxBackColor = Colors.DimGray,
                TextBoxBorderStyle = AGSBorders.SolidColor(glUtils, settings, Colors.Black, 1f),
                CheckboxCheckedAnimation = new ButtonAnimation(buttonBorder, null, Colors.SlateGray),
                CheckboxNotCheckedAnimation = new ButtonAnimation(buttonBorder, null, Colors.DimGray),
                CheckboxHoverCheckedAnimation = new ButtonAnimation(buttonBorder, null, Colors.LightGray),
                CheckboxHoverNotCheckedAnimation = new ButtonAnimation(buttonBorder, null, Colors.LightGray),
                DialogBoxColor = Colors.DarkGray,
                DialogBoxBorder = AGSBorders.SolidColor(glUtils, settings, Colors.Black, 2f)
            };
        }

        public ISkin CreateSkin()
        {
            return _skin.CreateSkin();
        }
    }
}
