using System;
using AGS.API;
using SkiaSharp;

namespace AGS.Engine
{
    public static class SkiaExtensions
    {
        public static Color Convert(this SKColor color) => Color.FromArgb(color.Alpha, color.Red, color.Green, color.Blue);

        public static SKColor Convert(this Color color) => new SKColor(color.Value);

        public static SKRectI Convert(this Rectangle rect) => new SKRectI(rect.X, rect.Y, rect.X + rect.Width, rect.Y + rect.Height);

        public static SKTypefaceStyle Convert(this FontStyle style)
        {
            bool isBold = style.HasFlag(FontStyle.Bold);
            bool isItalic = style.HasFlag(FontStyle.Italic);
            if (isBold && isItalic) return SKTypefaceStyle.BoldItalic;
            if (isBold) return SKTypefaceStyle.Bold;
            if (isItalic) return SKTypefaceStyle.Italic;
            return SKTypefaceStyle.Normal;
        }

        public static FontStyle Convert(this SKTypefaceStyle style)
        {
            switch (style)
            {
                case SKTypefaceStyle.Bold: return FontStyle.Bold;
                case SKTypefaceStyle.Italic: return FontStyle.Italic;
                case SKTypefaceStyle.BoldItalic: return FontStyle.Bold | FontStyle.Italic;
                default: return FontStyle.Regular;
            }
        }
    }
}