using AGS.API;
using AGS.Engine;

namespace AGS.Editor
{
    public static class GameViewColors
    {
        private static IGame _game = AGSGame.Game;
        private static IFontFactory fonts() => _game.Factory.Fonts;
        private static IBorderFactory borders() => _game.Factory.Graphics.Borders;

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
        public static Color Menu = Border;
        public static Color HoveredMenuItem = Colors.DarkBlue;
        public static Color TextEditor = Color.FromHexa(0x2d323a).WithAlpha(255);
        public static Color HoveredTextEditor = Color.FromHexa(0x505660).WithAlpha(255);
        public static Color PushedTextEditor = Color.FromHexa(0x282b30).WithAlpha(255);
        public static IBorderStyle TextEditorBorder = borders().SolidColor(TextEditor, 3f, true);
        public static IBorderStyle HoveredTextEditorBorder = borders().SolidColor(HoveredTextEditor, 3f, true);
        public static IBorderStyle DropDownBorder = borders().SolidColor(PushedTextEditor, 3f, true);
        public static IBorderStyle ComboboxTextBorder = borders().Gradient(new FourCorners<Color>(TextEditor),
                                                                           new FourCorners<bool>(true, false, true, false), 3f);
        public static IBorderStyle HoveredComboboxTextBorder = borders().Gradient(new FourCorners<Color>(HoveredTextEditor),
                                                                           new FourCorners<bool>(true, false, true, false), 3f);
        public static IBorderStyle ComboboxButtonBorder = borders().Gradient(new FourCorners<Color>(TextEditor),
                                                                           new FourCorners<bool>(false, true, false, true), 3f);
        public static IBorderStyle HoveredComboboxButtonBorder = borders().Gradient(new FourCorners<Color>(HoveredTextEditor),
                                                                           new FourCorners<bool>(false, true, false, true), 3f);

        public static IBrush TextBrush = _game.Factory.Graphics.Brushes.LoadSolidBrush(Text);
        public static IBrush HoveredTextBrush = _game.Factory.Graphics.Brushes.LoadSolidBrush(HoveredText);
        public static IBrush ReadonlyTextBrush = _game.Factory.Graphics.Brushes.LoadSolidBrush(ReadonlyText);

        public static ITextConfig ButtonTextConfig = fonts().GetTextConfig(autoFit: AutoFit.LabelShouldFitText, brush: TextBrush);
        public static ITextConfig TextboxTextConfig = fonts().GetTextConfig(autoFit: AutoFit.TextShouldCrop, brush: TextBrush);
        public static ITextConfig ReadonlyTextConfig = fonts().GetTextConfig(autoFit: AutoFit.LabelShouldFitText, brush: ReadonlyTextBrush,
             font: _game.Factory.Fonts.LoadFont(_game.Settings.Defaults.Fonts.Text.FontFamily, 12f));
        public static ITextConfig ButtonHoverTextConfig = fonts().GetTextConfig(autoFit: AutoFit.LabelShouldFitText, brush: HoveredTextBrush);
        public static ITextConfig TextboxHoverTextConfig = fonts().GetTextConfig(autoFit: AutoFit.TextShouldCrop, brush: HoveredTextBrush);
        public static ITextConfig ComboboxTextConfig = fonts().GetTextConfig(autoFit: AutoFit.LabelShouldFitText, brush: TextBrush, alignment: Alignment.MiddleLeft, labelMinSize: (100f, 25f));
        public static ITextConfig ComboboxHoverTextConfig = fonts().GetTextConfig(autoFit: AutoFit.LabelShouldFitText, brush: HoveredTextBrush, alignment: Alignment.MiddleLeft, labelMinSize: (100f, 25f));

        public static void AddHoverEffect(ITextBox textbox, 
                                          IBorderStyle border = null, IBorderStyle hoveredBorder = null,
                                          ITextConfig textConfig = null, ITextConfig hoveredTextConfig = null)
        {
            textbox.TextBackgroundVisible = true;
            textbox.Tint = TextEditor;
            border = border ?? TextEditorBorder;
            hoveredBorder = hoveredBorder ?? HoveredTextEditorBorder;
            textConfig = textConfig ?? TextboxTextConfig;
            hoveredTextConfig = hoveredTextConfig ?? TextboxHoverTextConfig;
            textbox.Border = border;
            textbox.TextConfig = textConfig;
            var uiEvents = textbox.GetComponent<IUIEvents>();
            uiEvents.MouseEnter.Subscribe(_ =>
            {
                textbox.TextConfig = hoveredTextConfig;
                textbox.Tint = HoveredTextEditor;
                textbox.Border = hoveredBorder;
            });
            uiEvents.MouseLeave.Subscribe(_ => 
            {
                textbox.TextConfig = textConfig;
                textbox.Tint = TextEditor;
                textbox.Border = border;
            });
        }
    }
}
