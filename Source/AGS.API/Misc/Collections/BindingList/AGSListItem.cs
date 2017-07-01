namespace AGS.API
{
    /// <summary>
    /// Represents an item in a list
    /// </summary>
    public struct AGSListItem<TItem>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="T:AGS.API.AGSListItem`1"/> struct.
        /// </summary>
        /// <param name="item">Item.</param>
        /// <param name="index">Index.</param>
        public AGSListItem(TItem item, int index)
        {
            Item = item;
            Index = index;
        }

        /// <summary>
        /// Gets the item.
        /// </summary>
        /// <value>The item.</value>
        public TItem Item { get; private set; }

        /// <summary>
        /// Gets the index of the item in the list.
        /// </summary>
        /// <value>The index.</value>
        public int Index { get; private set; }
    }
}
