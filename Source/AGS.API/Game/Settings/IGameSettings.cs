namespace AGS.API
{
    /// <summary>
    /// Vsync mode- how the FPS and screen refresh rate are synchronized.
    /// </summary>
	public enum VsyncMode
	{
        /// <summary>
        /// Vsync is off, so no synchronization is made between the FPS and the screen refresh rate.
        /// This means you will get tearing if frame rate is above your refresh rate.
        /// </summary>
		Off,
        /// <summary>
        /// Vsync is on, so you will get no vertical tearing. However if your frame rate is below the 
        /// screen refresh rate, you can get much lower FPS than you really should (unless you run with triple buffering,
        /// then this would not be a problem- triple buffering can be configured in the hardware driver, though using triple buffers mean more VRAM is being used).
        /// </summary>
		On,
	}

    /// <summary>
    /// Window state (minimized, maximized, full screen, floating).
    /// </summary>
	public enum WindowState
	{
        /// <summary>
        /// The window will be a normal floating window (will be floating on devices that support that).
        /// This is the default.
        /// </summary>
		Normal,
        /// <summary>
        /// The window will be minimized.
        /// </summary>
        Minimized,
        /// <summary>
        /// The window will be maximized. Unlike FullScreen mode, the computer's resolution will remain untouched
        /// and switching to other applications is easier. The window's border will still be seen (unless you
        /// set the window border to be hidden).
        /// </summary>
		Maximized,
        /// <summary>
        /// The window will in full screen mode, meaning the game will take over the computer and change the
        /// resolution to fit. No border will be shown regardless of the window border configuration.
        /// </summary>
		FullScreen,
	}

    /// <summary>
    /// Window border- resizable, fixed or hidden.
    /// </summary>
    public enum WindowBorder
    {
        /// <summary>
        /// The window border will be visible and the user will be able to resize the window by dragging the border (the default).
        /// </summary>
        Resizable,
        /// <summary>
        /// The window border will be visible but the user will not be able to resize it.
        /// </summary>
        Fixed,
        /// <summary>
        /// The window border will be hidden (recommended if you want to have your game at full screen, but without
        /// using the FullScreen mode that changes the computer's resolution).
        /// </summary>
        Hidden,
    }

    /// <summary>
    /// The graphics backend used to render the game.
    /// </summary>
    public enum GraphicsBackend : byte
    {
        /// <summary>
        /// Direct3D 11.
        /// </summary>
        Direct3D11,
        /// <summary>
        /// Vulkan.
        /// </summary>
        Vulkan,
        /// <summary>
        /// OpenGL.
        /// </summary>
        OpenGL,
        /// <summary>
        /// Metal.
        /// </summary>
        Metal,
        /// <summary>
        /// OpenGL ES.
        /// </summary>
        OpenGLES
    }

    /// <summary>
    /// Game settings (those can be set only once).
    /// </summary>
	public interface IGameSettings
	{
        /// <summary>
        /// Gets the title (the name of the game): would appear on the game window's title bar.
        /// </summary>
        /// <value>The title.</value>
		string Title { get; }
        /// <summary>
        /// Gets the virtual resolution of the game.
        /// This is the resolution in which you write your co-ordinates and sizes, and those will be 
        /// scaled appropriately to match the actual screen size.
        /// For example: let's say your virtual resolution is (320,200) and you have an object with width = 20,
        /// draw with bottom left (x,y) = (100,50). If the actual screen size is currently (640,400) that would
        /// render the object at actual screen (x,y) = (200,100) and the object will be scaled x2 to have width = 40.
        /// Note: you can override this resolution for specific layers, so you can have (as an example) hi-res UI
        /// in a low-res game.
        /// </summary>
        /// <value>The virtual resolution.</value>
		Size VirtualResolution { get; }
        /// <summary>
        /// Gets the size of the window (the size of the actual window on screen).
        /// </summary>
        /// <value>The size of the window.</value>
		Size WindowSize { get; }
        /// <summary>
        /// If the game is configured to preserve the aspect ratio, then in case the window is resized
        /// and the aspect ratio is changed, the screen will be letterboxed or pillarboxed so the
        /// aspect ratio for the actual content will remain the same (preserve aspect ratio is on by default).
        /// </summary>
        bool PreserveAspectRatio { get; }
        /// <summary>
        /// Gets the state of the window (minimized/maximized/etc).
        /// </summary>
        /// <value>The state of the window.</value>
		WindowState WindowState { get; }
        /// <summary>
        /// Gets the style of the window's border.
        /// </summary>
        /// <value>The window border.</value>
        WindowBorder WindowBorder { get; }
        /// <summary>
        /// Gets the vsync mode (synchronization of the frame update rate with screen refresh rate to prevent tearing).
        /// To read about how this works: https://hardforum.com/threads/how-vsync-works-and-why-people-loathe-it.928593/
        /// </summary>
        /// <value>The vsync.</value>
		VsyncMode Vsync { get; }
        /// <summary>
        /// The settings for various defaults.
        /// </summary>
        /// <value>The defaults.</value>
        IDefaultsSettings Defaults { get; }
        /// <summary>
        /// The graphics backend that renders the game.
        /// </summary>
        /// <value>The backend.</value>
        GraphicsBackend Backend { get; }
    }
}