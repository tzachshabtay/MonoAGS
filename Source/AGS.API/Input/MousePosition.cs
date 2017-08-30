using System;

namespace AGS.API
{
    /// <summary>
    /// Represents a mouse position on the screen.
    /// Allows to get the position in either window coordinates or viewport coordinates.
    /// </summary>
    public struct MousePosition
    {
        public static Size VirtualResolution { private get; set; }
        public static Func<int> GetWindowWidth { private get; set; }
        public static Func<int> GetWindowHeight { private get; set; }

        private IViewport _mainViewport;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:AGS.API.MousePosition"/> struct.
        /// </summary>
        /// <param name="xWindow">X position of the mouse in window coordinates.</param>
        /// <param name="yWindow">Y position of the mouse in window coordinates.</param>
        /// <param name="viewport">The main viewport.</param>
        public MousePosition(float xWindow, float yWindow, IViewport viewport)
        {
            XWindow = xWindow;
            YWindow = yWindow;
            _mainViewport = viewport;
        }

        /// <summary>
        /// X position of the mouse in window coordinates.
        /// Usually this will be less helpful for interacting with the world, for that you'd want to use the viewport coordinates.
        /// </summary>
        public float XWindow { get; private set; }

		/// <summary>
		/// Y position of the mouse in window coordinates.
		/// Usually this will be less helpful for interacting with the world, for that you'd want to use the viewport coordinates.
		/// </summary>
		public float YWindow { get; private set; }

        /// <summary>
        /// X position of the mouse in the main viewport coordinates (i.e the world coordinates).
        /// </summary>
        public float XMainViewport { get { return GetViewportX(_mainViewport); } }

        /// <summary>
        /// Y position of the mouse in the main viewport coordinates (i.e the world coordinates).
        /// </summary>
        public float YMainViewport { get { return GetViewportY(_mainViewport); } }

        /// <summary>
        /// Gets the x position of the mouse in a specific viewport coordinates.
        /// </summary>
        /// <returns>The x position.</returns>
        /// <param name="viewport">Viewport.</param>
        public float GetViewportX(IViewport viewport)
        {
            float windowWidth = GetWindowWidth();
            var projectLeft = viewport.ProjectionBox.X * windowWidth;
            var projectRight = projectLeft + viewport.ProjectionBox.Width * windowWidth;
            var virtualWidth = VirtualResolution.Width / viewport.ScaleX;
            float x = MathUtils.Lerp(projectLeft, 0f, projectRight, virtualWidth, XWindow);
			return x + viewport.X;
        }

        /// <summary>
        /// Gets the y position of the mouse in a specific viewport coordinates.
        /// </summary>
        /// <returns>The y position.</returns>
        /// <param name="viewport">Viewport.</param>
        public float GetViewportY(IViewport viewport)
        {
            float windowHeight = GetWindowHeight();
            var projectBottom = windowHeight - viewport.ProjectionBox.Y * windowHeight;
            var projectTop = projectBottom - viewport.ProjectionBox.Height * windowHeight;
            var virtualHeight = VirtualResolution.Height / viewport.ScaleY;
            float y = MathUtils.Lerp(projectTop, virtualHeight, projectBottom, 0f, YWindow);
			return y + viewport.Y;
        }
    }
}
