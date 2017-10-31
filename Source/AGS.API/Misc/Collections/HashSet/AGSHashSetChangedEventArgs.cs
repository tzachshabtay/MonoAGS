using System.Collections.Generic;

namespace AGS.API
{
    /// <summary>
    /// Event arguments for a hash set change event.
    /// </summary>
    public class AGSHashSetChangedEventArgs<TItem>
    {
        private IEnumerable<TItem> _items;
        private TItem _singleItem;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:AGS.API.AGSHashSetChangedEventArgs`1"/> class.
        /// </summary>
        /// <param name="changeType">Change type.</param>
        /// <param name="item">Item.</param>
        public AGSHashSetChangedEventArgs(ListChangeType changeType, TItem item)
        {
            ChangeType = changeType;
            _singleItem = item;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:AGS.API.AGSHashSetChangedEventArgs`1"/> class.
        /// </summary>
        /// <param name="changeType">Change type.</param>
        /// <param name="items">Items.</param>
        public AGSHashSetChangedEventArgs(ListChangeType changeType, IEnumerable<TItem> items)
        {
            ChangeType = changeType;
            _items = items;
        }

        /// <summary>
        /// Was an item added or removed from the set?
        /// </summary>
        /// <value>The type of the change.</value>
        public ListChangeType ChangeType { get; private set; }

        /// <summary>
        /// Gets the items which were involved in the change (either added or removed depending on <see cref="ChangeType"/>).
        /// </summary>
        /// <value>The item.</value>
        public IEnumerable<TItem> Items
        {
            get
            {
                if (_items != null) return _items;
                return getSingleItem();
            }
        }

        private IEnumerable<TItem> getSingleItem() { yield return _singleItem; }
    }
}
