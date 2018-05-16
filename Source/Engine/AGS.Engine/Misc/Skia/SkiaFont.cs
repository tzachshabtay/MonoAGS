using System;
using AGS.API;
using SkiaSharp;

namespace AGS.Engine
{
    public class SkiaFont : IFont
    {
        public static SkiaFont Create(string fontFamily, FontStyle style, float size)
        {
            var typeface = SKTypeface.FromFamilyName(fontFamily, style.Convert());
            var paint = new SKPaint { IsAntialias = true, SubpixelText = true, TextSize = size, Typeface = typeface };
            return new SkiaFont(typeface, style, size);
        }

        public static SkiaFont CreateFromPath(string path, FontStyle style, float size)
        {
            var typeface = SKTypeface.FromFile(path);
            typeface = SKTypeface.FromTypeface(typeface, style.Convert());
            return new SkiaFont(typeface, style, size);
        }

        public SkiaFont(SKTypeface typeface, FontStyle style, float size): this(
            new SKPaint { IsAntialias = true, SubpixelText = true, TextSize = size, Typeface = typeface },
            style.HasFlag(FontStyle.Underline),
            style.HasFlag(FontStyle.Strikeout))
        {
        }

        public SkiaFont(SKPaint paint, bool isUnderline = false, bool isStrikeout = false)
        {
            Paint = paint;
            FontFamily = paint.Typeface.FamilyName;
            SizeInPoints = paint.TextSize;

            var style = paint.Typeface.Style.Convert();
            if (isUnderline) style = style | FontStyle.Underline;
            if (isStrikeout) style = style | FontStyle.Strikeout;
            Style = style;
        }

        //todo: underline and strikethrough- need to draw those ourselves -> https://github.com/mono/SkiaSharp/issues/391
        public bool IsUnderline { get; }
        public bool IsStrikeout { get; }

        public SKPaint Paint { get; }

        public string FontFamily { get; }

        public FontStyle Style { get; }

        public float SizeInPoints { get; }

        public SizeF MeasureString(string text, int maxWidth = int.MaxValue)
        {
            //todo
            //_paint.BreakText(text, maxWidth, out var measuredWidth, out var measuredText);

            SKRect rect = new SKRect();
            Paint.MeasureText(text, ref rect);
            return new SizeF(rect.Width, rect.Height);
        }
    }
}