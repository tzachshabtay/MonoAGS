namespace AGS.API
{
    /// <summary>
    /// Event arguments for when a selected item is in the process of changing (before actually being changed) in a listbox/combobox.
    /// The event gives the ability to cancel the selection by settings <see cref="ShouldCancel"/> to true.
    /// </summary>
    public class ListboxItemChangingArgs : ListboxItemArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="T:AGS.API.ListboxItemChangingArgs"/> class.
        /// </summary>
        /// <param name="item">Item.</param>
        /// <param name="index">Index.</param>
        public ListboxItemChangingArgs(IStringItem item, int index) : base(item, index)
        {
        }

        /// <summary>
        /// A subscriber of the event can canel processing the item selection by setting this property to true.
        /// </summary>
        /// <value><c>true</c> if should cancel; otherwise, <c>false</c>.</value>
        public bool ShouldCancel { get; set; }
    }
}
