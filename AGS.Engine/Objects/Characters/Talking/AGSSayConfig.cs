using System;
using AGS.API;


namespace AGS.Engine
{
	public class AGSSayConfig : ISayConfig
	{ 
		public AGSSayConfig()
		{
			TextConfig = new AGSTextConfig(font : AGSGameSettings.DefaultSpeechFont, autoFit: AutoFit.TextShouldWrap);
			TextDelay = 70;
			LabelSize = new AGS.API.SizeF (250f, 200f);
			SkipText = SkipText.ByTimeAndMouse;
		}

		#region ISayConfig implementation

		public ITextConfig TextConfig { get; set; }

		public int TextDelay { get; set; }

		public SkipText SkipText { get; set; }

		public AGS.API.SizeF LabelSize { get; set; }

		#endregion
	}
}

