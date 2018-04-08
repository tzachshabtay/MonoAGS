using System;
using AGS.API;
using AGS.Engine;

namespace AGS.Editor
{
    public static class GameViewColors
    {
        public static Color Panel = Color.FromRgba(53, 64, 81, 250);
        public static Color Border = Color.FromRgba(44, 51, 61, 255);
        public static Color Text = Colors.WhiteSmoke;
        public static Color HoveredText = Colors.Yellow;
        public static Color ReadonlyText = Colors.LightGray;
        public static Color Button = Border;
        public static Color HoveredButton = Colors.Gray;
        public static Color PushedButton = Colors.DarkGray;
        public static Color SubPanel = Border;
        public static Color Textbox = Border;

        public static IBrush TextBrush = AGSGame.Game.Factory.Graphics.Brushes.LoadSolidBrush(Text);
        public static IBrush HoveredTextBrush = AGSGame.Game.Factory.Graphics.Brushes.LoadSolidBrush(HoveredText);
        public static IBrush ReadonlyTextBrush = AGSGame.Game.Factory.Graphics.Brushes.LoadSolidBrush(ReadonlyText);

        public static ITextConfig TextConfig = new AGSTextConfig(autoFit: AutoFit.LabelShouldFitText, brush: TextBrush);
        public static ITextConfig ReadonlyTextConfig = new AGSTextConfig(autoFit: AutoFit.LabelShouldFitText, brush: ReadonlyTextBrush, 
                    font: AGSGame.Device.FontLoader.LoadFont(AGSGameSettings.DefaultTextFont.FontFamily, 12f));
        public static ITextConfig HoverTextConfig = new AGSTextConfig(autoFit: AutoFit.LabelShouldFitText, brush: HoveredTextBrush);


        public static void AddHoverEffect(ITextBox textbox)
        {
            var uiEvents = textbox.GetComponent<IUIEvents>();
            uiEvents.MouseEnter.Subscribe(_ => textbox.TextConfig = HoverTextConfig);
            uiEvents.MouseLeave.Subscribe(_ => textbox.TextConfig = TextConfig);
        }
    }
}
