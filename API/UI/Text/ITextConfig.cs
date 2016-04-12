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
		IBrush Brush { get; }
		IFont Font { get; }
		Alignment Alignment { get; }

		IBrush OutlineBrush { get; }
		float OutlineWidth { get; }

		IBrush ShadowBrush  { get; }
		float ShadowOffsetX { get; }
		float ShadowOffsetY { get; }

		AutoFit AutoFit { get; }
		float PaddingLeft { get; }
		float PaddingRight { get; }
		float PaddingTop { get; }
		float PaddingBottom { get; }
	}
}

