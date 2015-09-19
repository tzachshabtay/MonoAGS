using System;
using AGS.API;
using System.Drawing;

namespace AGS.Engine
{
	public class AGSTextConfig : ITextConfig
	{
		public AGSTextConfig(Brush brush = null, Font font = null, Brush outlineBrush = null, float outlineWidth = 0f,
			Brush shadowBrush = null, float shadowOffsetX = 0f, float shadowOffsetY = 0f)
		{
			Brush = brush ?? Brushes.White;
			Font = font ?? DefaultFont;
			OutlineBrush = outlineBrush;
			OutlineWidth = outlineWidth;
			ShadowBrush = shadowBrush;
			ShadowOffsetX = shadowOffsetX;
			ShadowOffsetY = shadowOffsetY;
		}

		public static Font DefaultFont = new Font(SystemFonts.DefaultFont.FontFamily
			, 14f, FontStyle.Regular);

		#region ITextConfig implementation

		public Brush Brush { get; private set; }

		public Font Font { get; private set; }

		public Brush OutlineBrush { get; private set; }

		public float OutlineWidth { get; private set; }

		public Brush ShadowBrush { get; private set; }

		public float ShadowOffsetX { get; private set; }
		public float ShadowOffsetY { get; private set; }

		#endregion
	}
}

