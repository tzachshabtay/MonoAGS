using System;
using AGS.API;

namespace AGS.Editor
{
    public static class FontIcons
    {
        public static void Init(IFontLoader fontLoader)
        {
            Font = fontLoader.LoadFontFromPath("../../Assets/Fonts/Font Awesome 5 Free-Solid-900.otf", 3f, FontStyle.Regular);
        }

        public static IFont Font { get; private set; }

        //https://fontawesome.com/cheatsheet

        public const string ResizeHorizontal = "\uf337";
        public const string ResizeVertical = "\uf338";

        public const string RotateLeft = "\uf3e5";
        public const string RotateRight = "\uf064";

        public const string Move = "\uf062";
    }
}