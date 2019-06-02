using AGS.API;

namespace AGS.Engine
{
	public class AGSGameSettings : IGameSettings
	{
<<<<<<< HEAD
		public AGSGameSettings(string title, AGS.API.Size virtualResolution, WindowState windowState = WindowState.Maximized,
               AGS.API.Size? windowSize = null, VsyncMode vsync = VsyncMode.On, bool preserveAspectRatio = true,
               WindowBorder windowBorder = WindowBorder.Resizable, GraphicsBackend? backend = null)
=======
		public AGSGameSettings(string title, Size virtualResolution, WindowState windowState = WindowState.Maximized,
               Size? windowSize = null, VsyncMode vsync = VsyncMode.Adaptive, bool preserveAspectRatio = true,
               WindowBorder windowBorder = WindowBorder.Resizable)
>>>>>>> 9275f923b096bbb121ad033d1cd020cfe21cc5f4
		{
            Title = title;
            VirtualResolution = virtualResolution;
            WindowState = windowState;
            WindowSize = windowSize.HasValue ? windowSize.Value : virtualResolution;
            Vsync = vsync;
            PreserveAspectRatio = preserveAspectRatio;
            WindowBorder = windowBorder;
<<<<<<< HEAD
            Backend = backend ?? AGSGame.Device.GraphicsBackend.AutoDetect();
            Defaults = new AGSDefaultsSettings();
=======
            var fonts = new AGSDefaultFonts();
            var dialogs = new AGSDialogSettings(AGSGame.Device, fonts);
            Defaults = new AGSDefaultsSettings(fonts, dialogs);
>>>>>>> 9275f923b096bbb121ad033d1cd020cfe21cc5f4
		}

        public string Title { get; }
        
        public Size VirtualResolution { get; }

        public VsyncMode Vsync { get; }

        public Size WindowSize { get; }

        public bool PreserveAspectRatio { get; }

        public WindowState WindowState { get; }

        public WindowBorder WindowBorder { get; }

        public IDefaultsSettings Defaults { get; }

        public GraphicsBackend Backend { get; }
    }
}
