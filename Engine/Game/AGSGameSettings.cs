using System;
using System.Drawing;
using AGS.API;

namespace AGS.Engine
{
	public class AGSGameSettings : IGameSettings
	{
		public static Font DefaultSpeechFont = new Font(SystemFonts.DefaultFont.FontFamily
			, 14f, FontStyle.Regular);
		
		public static Font DefaultTextFont = new Font(SystemFonts.DefaultFont.FontFamily
			, 14f, FontStyle.Regular);

		public AGSGameSettings(string title, Size virtualResolution, WindowState windowState = WindowState.Maximized,
            Size? windowSize = null, VsyncMode vsync = VsyncMode.On)
		{
            Title = title;
            VirtualResolution = virtualResolution;
            WindowState = windowState;
            WindowSize = windowSize.HasValue ? windowSize.Value : virtualResolution;
            Vsync = vsync;
		}

        public string Title { get; private set; }
        
        public Size VirtualResolution { get; private set; }

        public VsyncMode Vsync { get; private set; }

        public Size WindowSize { get; private set; }

        public WindowState WindowState { get; private set; }
    }
}

