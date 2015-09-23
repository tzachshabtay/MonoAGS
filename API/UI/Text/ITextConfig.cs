using System;
using System.Drawing;

namespace AGS.API
{
	public enum AutoFit
	{
		NoFitting,
		LabelShouldFitText,
		TextShouldWrap,
		TextShouldFitLabel,
	}

	public interface ITextConfig
	{
		Brush Brush { get; }
		Font Font { get; }
		ContentAlignment Alignment { get; }

		Brush OutlineBrush { get; }
		float OutlineWidth { get; }

		Brush ShadowBrush  { get; }
		float ShadowOffsetX { get; }
		float ShadowOffsetY { get; }

		AutoFit AutoFit { get; }
		float PaddingLeft { get; }
		float PaddingRight { get; }
		float PaddingTop { get; }
		float PaddingBottom { get; }
	}
}

