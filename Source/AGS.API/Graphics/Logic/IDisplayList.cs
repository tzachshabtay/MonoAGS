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
        /// <param name="room">Room.</param>
        List<IObject> GetDisplayList(IRoom room);
    }
}
