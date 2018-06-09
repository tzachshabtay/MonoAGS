using AGS.API;
using SixLabors.Fonts;

namespace AGS.Engine
{
    public class ImageSharpFont : IFont
    {
        private readonly RendererOptions _noMaxRenderOptions;

        public ImageSharpFont(FontFamily family, float size, API.FontStyle style)
        {
            InnerFont = family.CreateFont(size, style.Convert());
            FontFamily = family.Name;
            Style = style;
            SizeInPoints = size;
            _noMaxRenderOptions = new RendererOptions(InnerFont);
        }

        public Font InnerFont { get; }

        public string FontFamily { get; }

        public API.FontStyle Style { get; }

        public float SizeInPoints { get; }

        public SizeF MeasureString(string text, int maxWidth = int.MaxValue)
        {
            var rendererOptions = maxWidth == int.MaxValue ? _noMaxRenderOptions : new RendererOptions(InnerFont) { WrappingWidth = maxWidth };
            return TextMeasurer.Measure(text, rendererOptions).Convert();
        }

        public IFont Resize(float sizeInPoints) => new ImageSharpFont(InnerFont.Family, sizeInPoints, Style);
    }
}