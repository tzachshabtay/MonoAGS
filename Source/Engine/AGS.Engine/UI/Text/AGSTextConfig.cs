using System.ComponentModel;
using AGS.API;


namespace AGS.Engine
{
    [PropertyFolder]
    public class AGSTextConfig : ITextConfig
    {
        private static IBrushLoader _brushes => AGSGame.Device.BrushLoader;

#pragma warning disable CS0067
        public event PropertyChangedEventHandler PropertyChanged;
#pragma warning restore CS0067

        public AGSTextConfig(IBrush brush = null, IFont font = null, IBrush outlineBrush = null, float outlineWidth = 0f,
            IBrush shadowBrush = null, float shadowOffsetX = 0f, float shadowOffsetY = 0f,
            Alignment alignment = Alignment.TopLeft, AutoFit autoFit = AutoFit.NoFitting,
            float paddingLeft = 2f, float paddingRight = 2f, float paddingTop = 2f, float paddingBottom = 2f)
        {
            Brush = brush ?? _brushes.LoadSolidBrush(Colors.White);
            Font = font ?? AGSGameSettings.DefaultTextFont;
            OutlineBrush = outlineBrush ?? _brushes.LoadSolidBrush(Colors.White);
            OutlineWidth = outlineWidth;
            ShadowBrush = shadowBrush;
            ShadowOffsetX = shadowOffsetX;
            ShadowOffsetY = shadowOffsetY;
            Alignment = alignment;
            AutoFit = autoFit;
            PaddingLeft = paddingLeft;
            PaddingRight = paddingRight;
            PaddingTop = paddingTop;
            PaddingBottom = paddingBottom;
        }

        public static AGSTextConfig Clone(ITextConfig config)
        {
            AGSTextConfig textConfig = new AGSTextConfig();
            textConfig.Brush = config.Brush;
            textConfig.Font = config.Font;
            textConfig.Alignment = config.Alignment;
            textConfig.OutlineBrush = config.OutlineBrush;
            textConfig.OutlineWidth = config.OutlineWidth;
            textConfig.ShadowBrush = config.ShadowBrush;
            textConfig.ShadowOffsetX = config.ShadowOffsetX;
            textConfig.ShadowOffsetY = config.ShadowOffsetY;
            textConfig.AutoFit = config.AutoFit;
            textConfig.PaddingLeft = config.PaddingLeft;
            textConfig.PaddingRight = config.PaddingRight;
            textConfig.PaddingTop = config.PaddingTop;
            textConfig.PaddingBottom = config.PaddingBottom;
            return textConfig;
        }

        public static AGSTextConfig ScaleConfig(ITextConfig config, float sizeFactor)
        {
            AGSTextConfig textConfig = Clone(config);
            textConfig.Font = AGSGame.Game.Factory.Fonts.LoadFont(config.Font.FontFamily, config.Font.SizeInPoints * sizeFactor, config.Font.Style);
            textConfig.OutlineWidth *= sizeFactor;
            textConfig.ShadowOffsetX *= sizeFactor;
            textConfig.ShadowOffsetY *= sizeFactor;
            textConfig.PaddingLeft *= sizeFactor;
            textConfig.PaddingRight *= sizeFactor;
            textConfig.PaddingTop *= sizeFactor;
            textConfig.PaddingBottom *= sizeFactor;
            return textConfig;
        }

        public static AGSTextConfig ChangeColor(ITextConfig config, Color color, Color outline, float outlineWidth)
        {
            AGSTextConfig textConfig = Clone(config);
            textConfig.Brush = _brushes.LoadSolidBrush(color);
            textConfig.OutlineBrush = _brushes.LoadSolidBrush(outline);
            textConfig.OutlineWidth = outlineWidth;
            return textConfig;
        }

        #region ITextConfig implementation

        public IBrush Brush { get; set; }

        public IFont Font { get; set; }

        public Alignment Alignment { get; set; }

        public IBrush OutlineBrush { get; set; }

        public float OutlineWidth { get; set; }

        public IBrush ShadowBrush { get; set; }

        public float ShadowOffsetX { get; set; }
        public float ShadowOffsetY { get; set; }

        public AutoFit AutoFit { get; set; }
        public float PaddingLeft { get; set; }
        public float PaddingRight { get; set; }
        public float PaddingTop { get; set; }
        public float PaddingBottom { get; set; }

        #endregion

        public override bool Equals(object obj) => Equals(obj as ITextConfig);

        public bool Equals(ITextConfig config)
        {
            if (config == null) return false;
            if (config == this) return true;
            return Brush == config.Brush && Font == config.Font && Alignment == config.Alignment
                  && OutlineBrush == config.OutlineBrush && MathUtils.FloatEquals(OutlineWidth, config.OutlineWidth)
                  && ShadowBrush == config.ShadowBrush && MathUtils.FloatEquals(ShadowOffsetX, config.ShadowOffsetX)
                  && MathUtils.FloatEquals(ShadowOffsetY, config.ShadowOffsetY) && AutoFit == config.AutoFit
                  && MathUtils.FloatEquals(PaddingLeft, config.PaddingLeft) && MathUtils.FloatEquals(PaddingTop, config.PaddingTop)
                  && MathUtils.FloatEquals(PaddingRight, config.PaddingRight) && MathUtils.FloatEquals(PaddingBottom, config.PaddingBottom);
        }

        public override int GetHashCode() => Font.GetHashCode();
    }
}

