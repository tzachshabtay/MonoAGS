using AGS.API;

namespace AGS.Engine
{
    public class AGSBlueSkin
    {
        private AGSColoredSkin _skin;

        public AGSBlueSkin(IGraphicsFactory factory)
        {
            _skin = new AGSColoredSkin(factory)
            {
                ButtonIdleBackColor = Colors.CornflowerBlue,
                ButtonHoverBackColor = Colors.Blue,
                ButtonPushedBackColor = Colors.DarkSlateBlue,
                ButtonBorderStyle = AGSBorders.SolidColor(Colors.DarkBlue, 1f),
                TextBoxBackColor = Colors.CornflowerBlue,                
                TextBoxBorderStyle = AGSBorders.SolidColor(Colors.DarkBlue, 1f),
                CheckboxCheckedColor = Colors.DarkSlateBlue,
                CheckboxNotCheckedColor = Colors.CornflowerBlue,
                CheckboxHoverCheckedColor = Colors.Blue,
                CheckboxHoverNotCheckedColor = Colors.Blue,
                CheckboxBorderStyle = AGSBorders.SolidColor(Colors.DarkBlue, 1f),
            };
        }

        public ISkin CreateSkin()
        {
            return _skin.CreateSkin();
        }
    }
}
