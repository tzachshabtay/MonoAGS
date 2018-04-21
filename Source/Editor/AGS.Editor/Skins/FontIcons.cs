using System;
using AGS.API;
using AGS.Engine;

namespace AGS.Editor
{
    public static class FontIcons
    {
        public static void Init(IFontLoader fontLoader)
        {
            const string path = "../../Assets/Fonts/Font Awesome 5 Free-Solid-900.otf";

            var smallFont = fontLoader.LoadFontFromPath(path, 3f, FontStyle.Regular);
            IconConfig = new AGSTextConfig(font: smallFont, autoFit: AutoFit.NoFitting, alignment: Alignment.MiddleCenter,
                                           paddingLeft: 0f, paddingTop: 0f, paddingBottom: 0f, paddingRight: 0f);

            var largeFont = fontLoader.LoadFontFromPath(path, 14f, FontStyle.Regular);
            ButtonConfig = new AGSTextConfig(font: largeFont, autoFit: AutoFit.TextShouldFitLabel, alignment: Alignment.MiddleCenter,
                                           paddingLeft: 0f, paddingTop: 0f, paddingBottom: 0f, paddingRight: 0f);
        }

        public static ITextConfig IconConfig { get; private set; }

        public static ITextConfig ButtonConfig { get; private set; }

        //https://fontawesome.com/cheatsheet

        public const string ResizeHorizontal = "\uf337";
        public const string ResizeVertical = "\uf338";

        public const string RotateLeft = "\uf3e5";
        public const string RotateRight = "\uf064";

        public const string Move = "\uf0b2";

        public const string Pivot = "\uf05b";

        public const string Pause = "\uf04c";
        public const string Play = "\uf04b";
    }
}