namespace AGS.API
{
    /// <summary>
    /// Event arguments for when a selected item changes in a combobox.
    /// </summary>
    public class ComboboxItemArgs : AGSEventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="T:AGS.API.ComboboxItemArgs"/> class.
        /// </summary>
        /// <param name="item">Item.</param>
        /// <param name="index">Index.</param>
        public ComboboxItemArgs(object item, int index)
        {
            Item = item;
            Index = index;
        }

        /// <summary>
        /// The newly selected item.
        /// </summary>
        /// <value>The item.</value>
        public object Item { get; private set; }

        /// <summary>
        /// The selected index.
        /// </summary>
        /// <value>The index.</value>
        public int Index { get; private set; }
    }
}
