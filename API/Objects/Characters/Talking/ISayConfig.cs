using System.Drawing;

namespace AGS.API
{
    public enum SkipText
	{
		ByTimeAndMouse,
		ByTime,
		ByMouse,
	}
		
	public interface ISayConfig
	{
		ITextConfig TextConfig { get; set; }
		int TextDelay { get; set; }
		SkipText SkipText { get; set; }
		SizeF LabelSize { get; set; }
	}
}

