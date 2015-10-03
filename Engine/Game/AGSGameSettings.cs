using System;
using System.Drawing;

namespace AGS.Engine
{
	public class AGSGameSettings
	{
		public static Font DefaultSpeechFont = new Font(SystemFonts.DefaultFont.FontFamily
			, 14f, FontStyle.Regular);
		
		public static Font DefaultTextFont = new Font(SystemFonts.DefaultFont.FontFamily
			, 14f, FontStyle.Regular);

		public AGSGameSettings()
		{
		}
	}
}

