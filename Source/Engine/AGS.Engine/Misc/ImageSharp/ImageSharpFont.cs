using System;
using AGS.API;
using SixLabors.Fonts;

namespace AGS.Engine
{
    public class ImageSharpFont : IFont
    {
        private readonly RendererOptions _noMaxRenderOptionsTopLeft;
        private readonly RendererOptions _noMaxRenderOptionsTopCenter;
        private readonly RendererOptions _noMaxRenderOptionsTopRight;
        private readonly RendererOptions _noMaxRenderOptionsMiddleLeft;
        private readonly RendererOptions _noMaxRenderOptionsMiddleCenter;
        private readonly RendererOptions _noMaxRenderOptionsMiddleRight;
        private readonly RendererOptions _noMaxRenderOptionsBottomLeft;
        private readonly RendererOptions _noMaxRenderOptionsBottomCenter;
        private readonly RendererOptions _noMaxRenderOptionsBottomRight;

        public ImageSharpFont(FontFamily family, float size, API.FontStyle style)
        {
            InnerFont = family.CreateFont(size, style.Convert());
            FontFamily = family.Name;
            Style = style;
            SizeInPoints = size;

            _noMaxRenderOptionsTopLeft = new RendererOptions(InnerFont) { VerticalAlignment = VerticalAlignment.Top, HorizontalAlignment = HorizontalAlignment.Left };
            _noMaxRenderOptionsTopCenter = new RendererOptions(InnerFont) { VerticalAlignment = VerticalAlignment.Top, HorizontalAlignment = HorizontalAlignment.Center };
            _noMaxRenderOptionsTopRight = new RendererOptions(InnerFont) { VerticalAlignment = VerticalAlignment.Top, HorizontalAlignment = HorizontalAlignment.Right };
            _noMaxRenderOptionsMiddleLeft = new RendererOptions(InnerFont) { VerticalAlignment = VerticalAlignment.Center, HorizontalAlignment = HorizontalAlignment.Left };
            _noMaxRenderOptionsMiddleCenter = new RendererOptions(InnerFont) { VerticalAlignment = VerticalAlignment.Center, HorizontalAlignment = HorizontalAlignment.Center };
            _noMaxRenderOptionsMiddleRight = new RendererOptions(InnerFont) { VerticalAlignment = VerticalAlignment.Center, HorizontalAlignment = HorizontalAlignment.Right };
            _noMaxRenderOptionsBottomLeft = new RendererOptions(InnerFont) { VerticalAlignment = VerticalAlignment.Bottom, HorizontalAlignment = HorizontalAlignment.Left };
            _noMaxRenderOptionsBottomCenter = new RendererOptions(InnerFont) { VerticalAlignment = VerticalAlignment.Bottom, HorizontalAlignment = HorizontalAlignment.Center };
            _noMaxRenderOptionsBottomRight = new RendererOptions(InnerFont) { VerticalAlignment = VerticalAlignment.Bottom, HorizontalAlignment = HorizontalAlignment.Right };
        }

        public Font InnerFont { get; }

        public string FontFamily { get; }

        public API.FontStyle Style { get; }

        public float SizeInPoints { get; }

        public SizeF MeasureString(string text, Alignment alignment, int maxWidth = int.MaxValue)
        {
            var rendererOptions = maxWidth == int.MaxValue ? getNoMaxRenderOptions(alignment) : getWithMaxRendererOptions(alignment, maxWidth);
            var rect = TextMeasurer.Measure(text, rendererOptions);
            return new SizeF(rect.Width, rect.Height);
        }

        public IFont Resize(float sizeInPoints) => new ImageSharpFont(InnerFont.Family, sizeInPoints, Style);

        private RendererOptions getNoMaxRenderOptions(Alignment alignment)
        {
            switch (alignment)
            {
                case Alignment.BottomCenter:
                    return _noMaxRenderOptionsBottomCenter;
                case Alignment.BottomLeft:
                    return _noMaxRenderOptionsBottomLeft;
                case Alignment.BottomRight:
                    return _noMaxRenderOptionsBottomRight;
                case Alignment.MiddleCenter:
                    return _noMaxRenderOptionsMiddleCenter;
                case Alignment.MiddleLeft:
                    return _noMaxRenderOptionsMiddleLeft;
                case Alignment.MiddleRight:
                    return _noMaxRenderOptionsMiddleRight;
                case Alignment.TopCenter:
                    return _noMaxRenderOptionsTopCenter;
                case Alignment.TopLeft:
                    return _noMaxRenderOptionsTopLeft;
                case Alignment.TopRight:
                    return _noMaxRenderOptionsTopRight;
                default:
                    throw new NotSupportedException(alignment.ToString());
            }
        }

        private RendererOptions getWithMaxRendererOptions(Alignment alignment, int maxWidth)
        {
            switch (alignment)
            {
                case Alignment.BottomCenter:
                    return new RendererOptions(InnerFont) { VerticalAlignment = VerticalAlignment.Bottom, HorizontalAlignment = HorizontalAlignment.Center, WrappingWidth = maxWidth };
                case Alignment.BottomLeft:
                    return new RendererOptions(InnerFont) { VerticalAlignment = VerticalAlignment.Bottom, HorizontalAlignment = HorizontalAlignment.Left, WrappingWidth = maxWidth };
                case Alignment.BottomRight:
                    return new RendererOptions(InnerFont) { VerticalAlignment = VerticalAlignment.Bottom, HorizontalAlignment = HorizontalAlignment.Right, WrappingWidth = maxWidth };
                case Alignment.MiddleCenter:
                    return new RendererOptions(InnerFont) { VerticalAlignment = VerticalAlignment.Center, HorizontalAlignment = HorizontalAlignment.Center, WrappingWidth = maxWidth };
                case Alignment.MiddleLeft:
                    return new RendererOptions(InnerFont) { VerticalAlignment = VerticalAlignment.Center, HorizontalAlignment = HorizontalAlignment.Left, WrappingWidth = maxWidth };
                case Alignment.MiddleRight:
                    return new RendererOptions(InnerFont) { VerticalAlignment = VerticalAlignment.Center, HorizontalAlignment = HorizontalAlignment.Right, WrappingWidth = maxWidth };
                case Alignment.TopCenter:
                    return new RendererOptions(InnerFont) { VerticalAlignment = VerticalAlignment.Top, HorizontalAlignment = HorizontalAlignment.Center, WrappingWidth = maxWidth };
                case Alignment.TopLeft:
                    return new RendererOptions(InnerFont) { VerticalAlignment = VerticalAlignment.Top, HorizontalAlignment = HorizontalAlignment.Left, WrappingWidth = maxWidth };
                case Alignment.TopRight:
                    return new RendererOptions(InnerFont) { VerticalAlignment = VerticalAlignment.Top, HorizontalAlignment = HorizontalAlignment.Right, WrappingWidth = maxWidth };
                default:
                    throw new NotSupportedException(alignment.ToString());
            }
        }
    }
}