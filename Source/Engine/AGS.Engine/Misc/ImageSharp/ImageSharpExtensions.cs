using System;
using AGS.API;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Drawing.Processing;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;

namespace AGS.Engine
{
    public static class ImageSharpExtensions
    {
        private static GraphicsOptions _clearOptions = new GraphicsOptions { AlphaCompositionMode = PixelAlphaCompositionMode.Src };

        public static void Clear(this Image<Rgba32> image, SolidBrush color)
        {
            image.Mutate(x => x.Fill(_clearOptions, color));
        }

        public static SixLabors.ImageSharp.Color Convert(this API.Color color) => new SixLabors.ImageSharp.Color(new Rgba32(color.Value));

        public static API.Color Convert(this SixLabors.ImageSharp.Color color) => API.Color.FromHexa(color.ToPixel<Rgba32>().PackedValue);

        public static API.Color Convert(this Rgba32 color) => API.Color.FromHexa(color.PackedValue);

        public static SixLabors.Fonts.FontStyle Convert(this FontStyle style)
        {
            bool isBold = style.HasFlag(FontStyle.Bold);
            bool isItalic = style.HasFlag(FontStyle.Italic);
            if (isBold && isItalic) return SixLabors.Fonts.FontStyle.BoldItalic;
            if (isBold) return SixLabors.Fonts.FontStyle.Bold;
            if (isItalic) return SixLabors.Fonts.FontStyle.Italic;
            return SixLabors.Fonts.FontStyle.Regular;
        }

        public static API.SizeF Convert(this SixLabors.ImageSharp.SizeF size)
        {
            return new API.SizeF(size.Width, size.Height);
        }
    }
}