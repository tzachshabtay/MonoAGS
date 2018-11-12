using AGS.API;

namespace AGS.Engine
{
	public class AGSGameSettings : IGameSettings
	{
		public AGSGameSettings(string title, Size virtualResolution, WindowState windowState = WindowState.Maximized,
               Size? windowSize = null, VsyncMode vsync = VsyncMode.Adaptive, bool preserveAspectRatio = true,
                               WindowBorder windowBorder = WindowBorder.Resizable)
		{
            Title = title;
            VirtualResolution = virtualResolution;
            WindowState = windowState;
            WindowSize = windowSize.HasValue ? windowSize.Value : virtualResolution;
            Vsync = vsync;
            PreserveAspectRatio = preserveAspectRatio;
            WindowBorder = windowBorder;
            Defaults = new AGSDefaultsSettings();
		}

        public string Title { get; }
        
        public Size VirtualResolution { get; }

        public VsyncMode Vsync { get; }

        public Size WindowSize { get; }

        public bool PreserveAspectRatio { get; }

        public WindowState WindowState { get; }

        public WindowBorder WindowBorder { get; }

        public IDefaultsSettings Defaults { get; }
    }
}
