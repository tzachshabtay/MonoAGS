using System.Drawing;

namespace AGS.API
{
	public enum VsyncMode
	{
		Off,
		On,
		Adaptive,
	}

	public enum WindowState
	{
		Normal,
		Maximized,
		FullScreen,
		Minimized,
	}

	public interface IGameSettings
	{
		string Title { get; }
		Size VirtualResolution { get; }
		Size WindowSize { get; }
		WindowState WindowState { get; }
		VsyncMode Vsync { get; }
	}
}