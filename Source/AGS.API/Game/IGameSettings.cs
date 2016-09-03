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
        /// <summary>
        /// If the game is configured to preserve the aspect ratio, then in case the window is resized
        /// and the aspect ratio is changed, the screen will be letterboxed or pillarboxed so the
        /// aspect ratio for the actual content will remain the same.
        /// </summary>
        bool PreserveAspectRatio { get; }
		WindowState WindowState { get; }
		VsyncMode Vsync { get; }
	}
}