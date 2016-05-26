using System;
using AGS.API;


namespace AGS.Engine
{
	public class AGSTextConfig : ITextConfig
	{
		public AGSTextConfig(IBrush brush = null, IFont font = null, IBrush outlineBrush = null, float outlineWidth = 0f,
			IBrush shadowBrush = null, float shadowOffsetX = 0f, float shadowOffsetY = 0f, 
			Alignment alignment = Alignment.TopLeft, AutoFit autoFit = AutoFit.NoFitting,
			float paddingLeft = 2f, float paddingRight = 2f, float paddingTop = 2f, float paddingBottom = 2f)
		{
			Brush = brush ?? Hooks.BrushLoader.LoadSolidBrush(Colors.White);
			Font = font ?? AGSGameSettings.DefaultTextFont;
			OutlineBrush = outlineBrush;
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

		public static AGSTextConfig FromConfig(ITextConfig config, float paddingBottomOffset = 0f)
		{
			AGSTextConfig textConfig = new AGSTextConfig ();
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
			textConfig.PaddingBottom = config.PaddingBottom + paddingBottomOffset;
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

