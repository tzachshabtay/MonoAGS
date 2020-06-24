using AGS.API;

namespace AGS.Engine
{
    public class AGSStringItem : IStringItem
    {
        public AGSStringItem(IFont font = null)
        {
            //todo: remove usage of AGSGame
            font = font ?? AGSGame.Game.Settings.Defaults.Fonts.Speech;
            IdleTextConfig = AGSGame.Game.Factory.Fonts.GetTextConfig(autoFit: AutoFit.LabelShouldFitText, font: font);
            HoverTextConfig = AGSTextConfig.ChangeColor(IdleTextConfig, Colors.Yellow, Colors.Black, 0f);
            Properties = new AGSCustomProperties();
        }

        public ITextConfig HoverTextConfig { get; set; }

        public ITextConfig IdleTextConfig { get; set; }

        public string Text { get; set; }

        public ICustomProperties Properties { get; private set; }
    }
}
