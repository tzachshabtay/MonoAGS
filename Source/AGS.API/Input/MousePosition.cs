using System;

namespace AGS.API
{
    /// <summary>
    /// Represents a mouse position on the screen.
    /// Allows to get the position in either window coordinates or viewport coordinates.
    /// </summary>
    public struct MousePosition
    {
        private readonly ICoordinates _coordinates;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:AGS.API.MousePosition"/> struct.
        /// </summary>
        /// <param name="xWindow">X position of the mouse in window coordinates.</param>
        /// <param name="yWindow">Y position of the mouse in window coordinates.</param>
        public MousePosition(float xWindow, float yWindow, ICoordinates coordinates)
        {
            XWindow = xWindow;
            YWindow = yWindow;
            _coordinates = coordinates;
            (XMainViewport, YMainViewport) = coordinates.WindowToWorld((xWindow, yWindow));
        }

        /// <summary>
        /// X position of the mouse in window coordinates.
        /// Usually this will be less helpful for interacting with the world, for that you'd want to use the viewport coordinates.
        /// </summary>
        public float XWindow { get; }

		/// <summary>
		/// Y position of the mouse in window coordinates.
		/// Usually this will be less helpful for interacting with the world, for that you'd want to use the viewport coordinates.
		/// </summary>
		public float YWindow { get; }

        /// <summary>
        /// X position of the mouse in the main viewport coordinates (i.e the world coordinates).
        /// </summary>
        public float XMainViewport { get; }

        /// <summary>
        /// Y position of the mouse in the main viewport coordinates (i.e the world coordinates).
        /// </summary>
        public float YMainViewport { get; }

        /// <summary>
        /// Gets a value indicating whether this <see cref="T:AGS.API.MousePosition"/> is inside the window.
        /// </summary>
        /// <value><c>true</c> if in window; otherwise, <c>false</c>.</value>
        public bool InWindow => _coordinates.IsWorldInWindow((XMainViewport, YMainViewport));

        /// <summary>
        /// Gets the x position of the mouse in a specific viewport coordinates.
        /// </summary>
        /// <returns>The x position.</returns>
        /// <param name="viewport">Viewport.</param>
        public float GetViewportX(IViewport viewport) => _coordinates.WindowXToViewportX(viewport, XWindow);

        /// <summary>
        /// Gets the y position of the mouse in a specific viewport coordinates.
        /// </summary>
        /// <returns>The y position.</returns>
        /// <param name="viewport">Viewport.</param>
        public float GetViewportY(IViewport viewport) => _coordinates.WindowYToViewportY(viewport, YWindow);

        /// <summary>
        /// Get the mouse position in a specific viewport coordinates, projected onto
        /// a specific object's world. 
        /// </summary>
        /// <returns>The projected point.</returns>
        /// <param name="viewport">Viewport.</param>
        /// <param name="projectedInto">Projected into.</param>
        public PointF GetProjectedPoint(IViewport viewport, IObject projectedInto) => 
            _coordinates.WindowToObject(viewport, projectedInto, (XWindow, YWindow));
    }
}