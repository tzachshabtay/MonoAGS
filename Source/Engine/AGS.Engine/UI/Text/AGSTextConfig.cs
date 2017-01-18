using AGS.API;


namespace AGS.Engine
{
	public class AGSTextConfig : ITextConfig
	{
        private static IBrushLoader _brushes { get { return AGSGame.Device.BrushLoader; } }

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

		public static AGSTextConfig FromConfig(ITextConfig config, float paddingBottomOffset = 0f)
		{
            AGSTextConfig textConfig = Clone(config);
            textConfig.PaddingBottom += paddingBottomOffset;
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

        public IBrush Brush { get; private set; }

		public IFont Font { get; private set; }

		public Alignment Alignment { get; private set; }

		public IBrush OutlineBrush { get; private set; }

		public float OutlineWidth { get; private set; }

		public IBrush ShadowBrush { get; private set; }

		public float ShadowOffsetX { get; private set; }
		public float ShadowOffsetY { get; private set; }

		public AutoFit AutoFit { get; private set; }
		public float PaddingLeft { get; private set; }
		public float PaddingRight { get; private set; }
		public float PaddingTop { get; private set; }
		public float PaddingBottom { get; private set; }

		#endregion
	}
}

