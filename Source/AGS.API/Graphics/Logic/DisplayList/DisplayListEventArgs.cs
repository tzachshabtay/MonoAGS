using System.Collections.Generic;

namespace AGS.API
{
    /// <summary>
    /// Event arguments for retrieiving (and possibly modifying) the display list before rendering it.
    /// </summary>
    public class DisplayListEventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="T:AGS.API.DisplayListEventArgs"/> class.
        /// </summary>
        /// <param name="displayList">Display list.</param>
        public DisplayListEventArgs(List<IObject> displayList)
        {
            DisplayList = displayList;
        }

        /// <summary>
        /// Gets or sets the display list.
        /// </summary>
        /// <value>The display list.</value>
        public List<IObject> DisplayList { get; set; }
    }
}
