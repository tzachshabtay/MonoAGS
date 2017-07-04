namespace AGS.API
{
    /// <summary>
    /// Event arguments for a hash set change event.
    /// </summary>
    public class AGSHashSetChangedEventArgs<TItem>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="T:AGS.API.AGSHashSetChangedEventArgs`1"/> class.
        /// </summary>
        /// <param name="changeType">Change type.</param>
        /// <param name="item">Item.</param>
        public AGSHashSetChangedEventArgs(ListChangeType changeType, TItem item)
        {
            ChangeType = changeType;
            Item = item;
        }

        /// <summary>
        /// Was an item added or removed from the set?
        /// </summary>
        /// <value>The type of the change.</value>
        public ListChangeType ChangeType { get; private set; }

        /// <summary>
        /// Gets the item which was involved in the change (either added or removed, <see cref="ChangeType"/> .
        /// </summary>
        /// <value>The item.</value>
        public TItem Item { get; private set; }
    }
}
