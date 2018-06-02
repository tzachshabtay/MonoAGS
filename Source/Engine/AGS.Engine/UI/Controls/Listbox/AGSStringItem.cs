using AGS.API;

namespace AGS.Engine
{
    public class AGSStringItem : IStringItem
    {
        public AGSStringItem(IFont font = null)
        {
            font = font ?? AGSGame.Game.Settings.Defaults.SpeechFont;
            IdleTextConfig = new AGSTextConfig(autoFit: AutoFit.LabelShouldFitText, font: font);
            HoverTextConfig = AGSTextConfig.ChangeColor(IdleTextConfig, Colors.Yellow, Colors.Black, 1f);
            Properties = new AGSCustomProperties();
        }

        public ITextConfig HoverTextConfig { get; set; }

        public ITextConfig IdleTextConfig { get; set; }

        public string Text { get; set; }

        public ICustomProperties Properties { get; private set; }
    }
}