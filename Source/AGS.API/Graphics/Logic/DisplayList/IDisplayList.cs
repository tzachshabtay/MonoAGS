using System.Collections.Generic;

namespace AGS.API
{
    /// <summary>
    /// Represents an ordered list of objects that will be displayed on screen.
    /// </summary>
    public interface IDisplayList
    {
		/// <summary>
		/// Gets an ordered list of objects to display on screen.
		/// </summary>
		/// <returns>The display list.</returns>
		/// <param name="viewport">Viewport.</param>
		List<IObject> GetDisplayList(IViewport viewport);

        /// <summary>
        /// Gets the mouse cursor.
        /// </summary>
        /// <returns>The cursor.</returns>
        IObject GetCursor();

        /// <summary>
        /// Update the display list (this is called every tick by the game engine itself).
        /// </summary>
        void Update();
    }
}
