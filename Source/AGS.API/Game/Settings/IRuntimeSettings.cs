using System;
using System.ComponentModel;

namespace AGS.API
{
    /// <summary>
    /// Game settings.
    /// </summary>
    public interface IRuntimeSettings : IGameSettings, INotifyPropertyChanged
    {
        /// <summary>
        /// Gets the title (the name of the game): would appear on the game window's title bar.
        /// </summary>
        /// <value>The title.</value>
        new string Title { get; set; }

        /// <summary>
        /// Gets the size of the window (the size of the actual window on screen).
        /// </summary>
        /// <value>The size of the window.</value>
        new Size WindowSize { get; set; }

        /// <summary>
        /// If the game is configured to preserve the aspect ratio, then in case the window is resized
        /// and the aspect ratio is changed, the screen will be letterboxed or pillarboxed so the
        /// aspect ratio for the actual content will remain the same  
        /// </summary>
        new bool PreserveAspectRatio { get; set; }

        /// <summary>
        /// Gets the state of the window (minimized/maximized/etc).
        /// </summary>
        /// <value>The state of the window.</value>
        new WindowState WindowState { get; set; }

        /// <summary>
        /// Gets the style of the window's border.
        /// </summary>
        /// <value>The window border.</value>
        new WindowBorder WindowBorder { get; set; }

        /// <summary>
        /// Gets the vsync mode (synchronization of the frame update rate with screen refresh rate to prevent tearing).
        /// To read about how this works: https://hardforum.com/threads/how-vsync-works-and-why-people-loathe-it.928593/
        /// </summary>
        /// <value>The vsync.</value>
        new VsyncMode Vsync { get; set; }

        /// <summary>
        /// Loads settings from the specified object.
        /// </summary>
        /// <param name="settings">Settings.</param>
        void LoadFrom(IGameSettings settings);
    }
}
