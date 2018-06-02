using AGS.API;

namespace AGS.Engine
{
    public class AGSBlueSkin
    {
        private AGSColoredSkin _skin;

        public AGSBlueSkin(IGraphicsFactory factory, IGLUtils glUtils, IGameSettings settings)
        {
            var buttonBorder = AGSBorders.SolidColor(glUtils, settings, Colors.DarkBlue, 1f);
            _skin = new AGSColoredSkin(factory)
            {
                ButtonIdleAnimation = new ButtonAnimation(buttonBorder, null, Colors.CornflowerBlue),
                ButtonHoverAnimation = new ButtonAnimation(buttonBorder, null, Colors.Blue),
                ButtonPushedAnimation = new ButtonAnimation(buttonBorder, null, Colors.DarkSlateBlue),
                TextBoxBackColor = Colors.CornflowerBlue,                
                TextBoxBorderStyle = AGSBorders.SolidColor(glUtils, settings, Colors.DarkBlue, 1f),
                CheckboxCheckedAnimation = new ButtonAnimation(buttonBorder, null, Colors.DarkSlateBlue),
                CheckboxNotCheckedAnimation = new ButtonAnimation(buttonBorder, null, Colors.CornflowerBlue),
                CheckboxHoverCheckedAnimation = new ButtonAnimation(buttonBorder, null, Colors.Blue),
                CheckboxHoverNotCheckedAnimation = new ButtonAnimation(buttonBorder, null, Colors.Blue),
                DialogBoxColor = Colors.DarkSlateBlue,
                DialogBoxBorder = AGSBorders.SolidColor(glUtils, settings, Colors.DarkBlue, 2f)
            };
        }

        public ISkin CreateSkin()
        {
            return _skin.CreateSkin();
        }
    }
}
