using AGS.API;
using AGS.Engine;

namespace AGS.Editor
{
    public static class FontIcons
    {
        public static void Init(IFontFactory fontLoader)
        {
            const string path = "Fonts/Font Awesome 5 Free-Solid-900.otf";

            Font = fontLoader.LoadFontFromPath(path, 14f, FontStyle.Regular);
            IconConfig = fontLoader.GetTextConfig(font: Font, autoFit: AutoFit.NoFitting, alignment: Alignment.MiddleCenter,
                                           paddingLeft: 0f, paddingTop: 0f, paddingBottom: 0f, paddingRight: 0f);

            ButtonConfig = fontLoader.GetTextConfig(font: Font, autoFit: AutoFit.TextShouldFitLabel, alignment: Alignment.MiddleCenter,
                                           paddingLeft: 0f, paddingTop: 0f, paddingBottom: 0f, paddingRight: 0f);

            var tinyFont = fontLoader.LoadFontFromPath(path, 8f, FontStyle.Regular);
            TinyButtonConfig = fontLoader.GetTextConfig(font: tinyFont, autoFit: AutoFit.TextShouldFitLabel, alignment: Alignment.MiddleCenter,
                                                 paddingLeft: -1f, paddingTop: 0f, paddingBottom: 0f, paddingRight: 0f, outlineWidth: 1f);

            TinyButtonConfigHovered = AGSTextConfig.ChangeColor(TinyButtonConfig, Colors.Black, Colors.White, 0f);
        }

        public static IFont Font { get; private set; }

        public static ITextConfig IconConfig { get; private set; }

        public static ITextConfig ButtonConfig { get; private set; }

        public static ITextConfig TinyButtonConfig { get; private set; }

        public static ITextConfig TinyButtonConfigHovered { get; private set; }

        //https://fontawesome.com/cheatsheet

        public const string ResizeHorizontal = "\uf337";
        public const string ResizeVertical = "\uf338";

        public const string RotateLeft = "\uf3e5";
        public const string RotateRight = "\uf064";

        public const string Move = "\uf0b2";
        public const string Pivot = "\uf05b";

        public const string Pause = "\uf04c";
        public const string Play = "\uf04b";

        public const string Pointer = "\uf245";

        public const string SubMenu = "\uf105";

        public const string CaretDown = "\uf0d7";
        public const string CaretUp = "\uf0d8";
        public const string CaretRight = "\uf0da";

        public const string Square = "\uf0c8";
        public const string CheckSquare = "\uf14a";

        public const string RadioUnchecked = "\uf111";
        public const string RadioChecked = "\uf192";
    }
}
