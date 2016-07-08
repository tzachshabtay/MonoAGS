namespace AGS.API
{
    public enum SkipText
	{
		ByTimeAndMouse,
		ByTime,
		ByMouse,
		External,
	}

    public interface ISayConfig
	{
		ITextConfig TextConfig { get; set; }
		int TextDelay { get; set; }
		SkipText SkipText { get; set; }
		SizeF LabelSize { get; set; }
		IBorderStyle Border { get; set; }
		Color BackgroundColor { get; set; }
        IPortraitConfig PortraitConfig { get; set; }
	}
}

