using System;
using System.ComponentModel;

namespace AGS.API
{
    /// <summary>
    /// Information about the application window and the sub-window containing the game.
    /// Usually, the window hosting the game IS the application window, but for some scenarios (like for the editor which is hosting the game)
    /// the game can be configured to only use part of the window (a "sub window").
    /// </summary>
    public interface IWindowInfo : INotifyPropertyChanged
    {
        /// <summary>
        /// Gets the height of the application window.
        /// </summary>
        /// <value>The height of the application window.</value>
        float AppWindowHeight { get; }

        /// <summary>
        /// Gets the width of the application window.
        /// </summary>
        /// <value>The width of the application window.</value>
        float AppWindowWidth { get; }

        /// <summary>
        /// Gets the rectangle (a subset of the application window) which contains the game.
        /// In the normal case this rectangle will be the entire application window.
        /// This rectangle includes possible black borders (if <see cref="IGameSettings.PreserveAspectRatio"/> is enabled).
        /// To get the rectangle without the black borders, <see cref="IViewport.ScreenArea"/>.
        /// </summary>
        /// <value>The game sub window.</value>
        Rectangle GameSubWindow { get; }
    }
}