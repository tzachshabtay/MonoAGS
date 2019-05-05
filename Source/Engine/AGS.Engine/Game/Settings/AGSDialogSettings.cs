using AGS.API;

namespace AGS.Engine
{
    public class AGSDialogSettings : IDialogSettings
    {
        private readonly IDevice _device;
        private readonly IDefaultFonts _defaultFonts;

        public AGSDialogSettings(IDevice device, IDefaultFonts defaultFonts)
        {
            _device = device;
            _defaultFonts = defaultFonts;

            Idle = getTextConfig(autoFit: AutoFit.TextShouldWrapAndLabelShouldFitHeight,
                brush: device.BrushLoader.LoadSolidBrush(Colors.White), font: new defaultFont(defaultFonts));

            Hovered = getTextConfig(autoFit: AutoFit.TextShouldWrapAndLabelShouldFitHeight,
                brush: device.BrushLoader.LoadSolidBrush(Colors.Yellow), font: new defaultFont(defaultFonts));

            Chosen = getTextConfig(autoFit: AutoFit.TextShouldWrapAndLabelShouldFitHeight,
                brush: device.BrushLoader.LoadSolidBrush(Colors.Gray), font: new defaultFont(defaultFonts));
        }

        public IRenderLayer RenderLayer { get; set; } = AGSLayers.Dialogs;
        public ITextConfig Idle { get; set; }
        public ITextConfig Hovered { get; set; }
        public ITextConfig Chosen { get; set; }

        //todo: share code with font factory
        private ITextConfig getTextConfig(IBrush brush = null, IFont font = null, IBrush outlineBrush = null, float outlineWidth = 0f,
            IBrush shadowBrush = null, float shadowOffsetX = 0f, float shadowOffsetY = 0f,
            Alignment alignment = Alignment.TopLeft, AutoFit autoFit = AutoFit.NoFitting,
            float paddingLeft = 2f, float paddingRight = 2f, float paddingTop = 2f, float paddingBottom = 2f, SizeF? labelMinSize = null)
        {
            return new AGSTextConfig
            {
                Brush = brush ?? _device.BrushLoader.LoadSolidBrush(Colors.White),
                Font = font ?? _defaultFonts.Text,
                OutlineBrush = outlineBrush ?? _device.BrushLoader.LoadSolidBrush(Colors.White),
                OutlineWidth = outlineWidth,
                ShadowBrush = shadowBrush,
                ShadowOffsetX = shadowOffsetX,
                ShadowOffsetY = shadowOffsetY,
                Alignment = alignment,
                AutoFit = autoFit,
                PaddingLeft = paddingLeft,
                PaddingRight = paddingRight,
                PaddingTop = paddingTop,
                PaddingBottom = paddingBottom,
                LabelMinSize = labelMinSize
            };
        }

        private class defaultFont : IFont
        {
            private IDefaultFonts _defaults;

            public defaultFont(IDefaultFonts defaults)
            {
                _defaults = defaults;
            }

            public string FontFamily => getFont().FontFamily;

            public FontStyle Style => getFont().Style;

            public float SizeInPoints => getFont().SizeInPoints;

            public SizeF MeasureString(string text, Alignment alignment, int maxWidth = int.MaxValue)
            {
                return getFont().MeasureString(text, alignment, maxWidth);
            }

            public IFont Resize(float sizeInPoints)
            {
                return getFont().Resize(sizeInPoints);
            }

            private IFont getFont() => _defaults.Dialogs ?? _defaults.Text;
        }
    }
}
