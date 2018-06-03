using System;
namespace AGS.API
{
    /// <summary>
    /// Utility methods to help convert between different coordinate systems:
    /// 
    /// 1. Window coordinates: those are the coordinates of the window which hosts the game (the units are based on the number of pixels the window has on your screen- determined from your screen resolution and display size).
    /// 2. World coordinates: those are the coordinates of your game world (the units are based on your virtual resolution).
    /// 3. Viewport coordinates: the game might have multiple viewports on the screen (for example, in a split-screen game). Each viewport can have its own coordinates. The world coordinates are actually the main viewport coordinates.
    /// 4. Object coordinates: each object in the game can have its own coordinates. First, it can be using a <see cref="IRenderLayer"/> which has a different resolution than your world's resolution. Secondly, if the object is a child of another object, then the coordinate system is relative to the parent.
    /// </summary>
    public interface ICoordinates
    {
        /// <summary>
        /// Converts a viewport X coordinate to the window X coordinate.
        /// </summary>
        /// <param name="viewport">Viewport.</param>
        /// <param name="x">The x coordinate.</param>
        float ViewportXToWindowX(IViewport viewport, float x);

        /// <summary>
        /// Converts a window X coordinate to the viewport X coordinate.
        /// </summary>
        /// <param name="viewport">Viewport.</param>
        /// <param name="x">The x coordinate.</param>
        float WindowXToViewportX(IViewport viewport, float x);

        /// <summary>
        /// Converts a viewport Y coordinate to the window Y coordinate.
        /// </summary>
        /// <param name="viewport">Viewport.</param>
        /// <param name="y">The y coordinate.</param>
        float ViewportYToWindowY(IViewport viewport, float y);

        /// <summary>
        /// Converts a window Y coordinate to the viewport Y coordinate.
        /// </summary>
        /// <param name="viewport">Viewport.</param>
        /// <param name="y">The y coordinate.</param>
        float WindowYToViewportY(IViewport viewport, float y);

        /// <summary>
        /// Converts a world X coordinate to the window X coordinate.
        /// </summary>
        /// <param name="x">The x coordinate.</param>
        float WorldXToWindowX(float x);

        /// <summary>
        /// Converts a world Y coordinate to the window Y coordinate.
        /// </summary>
        /// <param name="y">The y coordinate.</param>
        float WorldYToWindowY(float y);

        /// <summary>
        /// Converts a window X coordinate to the world X coordinate.
        /// </summary>
        /// <param name="x">The x coordinate.</param>
        float WindowXToWorldX(float x);

        /// <summary>
        /// Converts a window Y coordinate to the world Y coordinate.
        /// </summary>
        /// <param name="y">The y coordinate.</param>
        float WindowYToWorldY(float y);

        /// <summary>
        /// Converts viewport coordinates to window coordinates.
        /// </summary>
        /// <param name="viewport">Viewport.</param>
        /// <param name="position">Position.</param>
        PointF ViewportToWindow(IViewport viewport, PointF position);

        /// <summary>
        /// Converts window coordinates to viewport coordinates.
        /// </summary>
        /// <param name="viewport">Viewport.</param>
        /// <param name="position">Position.</param>
        PointF WindowToViewport(IViewport viewport, PointF position);

        /// <summary>
        /// Converts world coordinates to window coordinates.
        /// </summary>
        /// <param name="position">Position.</param>
        PointF WorldToWindow(PointF position);

        /// <summary>
        /// Converts window coordinates to world coordinates.
        /// </summary>
        /// <param name="position">Position.</param>
        PointF WindowToWorld(PointF position);

        /// <summary>
        /// Converts window coordinates to object coordinates.
        /// </summary>
        /// <param name="viewport">Viewport.</param>
        /// <param name="obj">Object.</param>
        /// <param name="position">Position.</param>
        PointF WindowToObject(IViewport viewport, IObject obj, PointF position);

        /// <summary>
        /// Checks if the given world position is inside the window.
        /// </summary>
        /// <returns><c>true</c>, if world position is inside the window, <c>false</c> otherwise.</returns>
        /// <param name="worldPosition">World position.</param>
        bool IsWorldInWindow(PointF worldPosition);
    }
}