using System.ComponentModel;

namespace AGS.API
{
    /// <summary>
    /// Controls what is shown on screen for this display list.
    /// </summary>
    public interface IDisplayListSettings : INotifyPropertyChanged
    {
        /// <summary>
        /// Should we display the objects in the room?
        /// </summary>
        /// <value><c>true</c> if display room; otherwise, <c>false</c>.</value>
        bool DisplayRoom { get; set; }

        /// <summary>
        /// Should we display the GUIs?
        /// </summary>
        /// <value><c>true</c> if display GUI; otherwise, <c>false</c>.</value>
        bool DisplayGUIs { get; set; }

        /// <summary>
        /// Allows fine grain control over which entities will be displayed.
        /// </summary>
        /// <value>The restriction list.</value>
        IRestrictionList RestrictionList { get; }

        /// <summary>
        /// Allows configuring depth clipping (i.e hiding objects if they are too close or too far from the camera).
        /// </summary>
        /// <value>The depth clipping.</value>
        IDepthClipping DepthClipping { get; }
    }
}
