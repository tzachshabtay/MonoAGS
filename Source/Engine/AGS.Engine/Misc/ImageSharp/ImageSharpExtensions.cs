using System;
using AGS.API;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp.Processing.Drawing;
using SixLabors.ImageSharp.Processing.Drawing.Brushes;

namespace AGS.Engine
{
    public static class ImageSharpExtensions
    {
        private static GraphicsOptions _clearOptions = new GraphicsOptions { BlenderMode = PixelBlenderMode.Src };

        public static void Clear(this Image<Rgba32> image, SolidBrush<Rgba32> color)
        {
            image.Mutate(x => x.Fill(_clearOptions, color));
        }

        public static Rgba32 Convert(this Color color) => new Rgba32(color.Value);

        public static Color Convert(this Rgba32 color) => Color.FromHexa(color.PackedValue);

        public static SixLabors.Fonts.FontStyle Convert(this FontStyle style)
        {
            bool isBold = style.HasFlag(FontStyle.Bold);
            bool isItalic = style.HasFlag(FontStyle.Italic);
            if (isBold && isItalic) return SixLabors.Fonts.FontStyle.BoldItalic;
            if (isBold) return SixLabors.Fonts.FontStyle.Bold;
            if (isItalic) return SixLabors.Fonts.FontStyle.Italic;
            return SixLabors.Fonts.FontStyle.Regular;
        }

        public static SizeF Convert(this SixLabors.Primitives.SizeF size)
        {
            return new SizeF(size.Width, size.Height);
        }
    }
}