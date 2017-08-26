namespace AGS.API
{
    /// <summary>
    /// Controls what is shown on screen for this display list.
    /// </summary>
    public interface IDisplayListSettings
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
        /// Should we display the mouse cursor?
        /// </summary>
        /// <value><c>true</c> if display cursor; otherwise, <c>false</c>.</value>
        bool DisplayCursor { get; set; }
    }
}
