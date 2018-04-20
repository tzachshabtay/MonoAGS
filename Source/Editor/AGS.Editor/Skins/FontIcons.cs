using System;
using AGS.API;
using AGS.Engine;

namespace AGS.Editor
{
    public static class FontIcons
    {
        public static void Init(IFontLoader fontLoader)
        {
            var font = fontLoader.LoadFontFromPath("../../Assets/Fonts/Font Awesome 5 Free-Solid-900.otf", 3f, FontStyle.Regular);
            IconConfig = new AGSTextConfig(font: font, autoFit: AutoFit.NoFitting, alignment: Alignment.MiddleCenter,
                                           paddingLeft: 0f, paddingTop: 0f, paddingBottom: 0f, paddingRight: 0f);
        }

        public static ITextConfig IconConfig { get; private set; }

        //https://fontawesome.com/cheatsheet

        public const string ResizeHorizontal = "\uf337";
        public const string ResizeVertical = "\uf338";

        public const string RotateLeft = "\uf3e5";
        public const string RotateRight = "\uf064";

        public const string Move = "\uf0b2";

        public const string Pivot = "\uf05b";
    }
}