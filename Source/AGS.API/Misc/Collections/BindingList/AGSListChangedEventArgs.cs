namespace AGS.API
{
    /// <summary>
    /// How did the list change?
    /// </summary>
	public enum ListChangeType
	{
        /// <summary>
        /// An item was added to the list.
        /// </summary>
		Add,
        /// <summary>
        /// An item was removed from the list.
        /// </summary>
		Remove,
	}

    /// <summary>
    /// Event arguments for a list change.
    /// </summary>
	public class AGSListChangedEventArgs<TItem> : AGSEventArgs
	{
        /// <summary>
        /// Initializes a new instance of the <see cref="T:AGS.API.AGSListChangedEventArgs`1"/> class.
        /// </summary>
        /// <param name="changeType">Change type.</param>
        /// <param name="item">Item.</param>
        /// <param name="index">Index.</param>
		public AGSListChangedEventArgs(ListChangeType changeType, TItem item, int index)
		{
			ChangeType = changeType;
			Item = item;
			Index = index;
		}

        /// <summary>
        /// How was the list changed?
        /// </summary>
        /// <value>The type of the change.</value>
		public ListChangeType ChangeType { get; private set; }

        /// <summary>
        /// Gets the item which was involved in the change (either added or removed depending on <see cref="ChangeType"/>).
        /// </summary>
        /// <value>The item.</value>
		public TItem Item { get; private set; }

        /// <summary>
        /// Gets the index of the item which was involved in the change (either added or removed depending on <see cref="ChangeType"/>).
        /// </summary>
        /// <value>The index.</value>
		public int Index { get; private set; }
	}
}

