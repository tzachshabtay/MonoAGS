using System.Collections.Generic;

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
	public class AGSListChangedEventArgs<TItem>
	{
        private IEnumerable<AGSListItem<TItem>> _items;
        private AGSListItem<TItem> _singleItem;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:AGS.API.AGSListChangedEventArgs`1"/> class.
        /// </summary>
        /// <param name="changeType">Change type.</param>
        /// <param name="item">Item.</param>
        public AGSListChangedEventArgs(ListChangeType changeType, AGSListItem<TItem> item)
		{
			ChangeType = changeType;
            _singleItem = item;
		}

        /// <summary>
        /// Initializes a new instance of the <see cref="T:AGS.API.AGSListChangedEventArgs`1"/> class.
        /// </summary>
        /// <param name="changeType">Change type.</param>
        /// <param name="items">Items.</param>
        public AGSListChangedEventArgs(ListChangeType changeType, IEnumerable<AGSListItem<TItem>> items)
        {
            ChangeType = changeType;
            _items = items;
        }

        /// <summary>
        /// How was the list changed?
        /// </summary>
        /// <value>The type of the change.</value>
		public ListChangeType ChangeType { get; private set; }

        /// <summary>
        /// Gets the items which were involved in the change (either added or removed depending on <see cref="ChangeType"/>).
        /// </summary>
        /// <value>The item.</value>
        public IEnumerable<AGSListItem<TItem>> Items
        {
            get
            {
                if (_items != null) return _items;
                return getSingleItem();
            }
        }

        private IEnumerable<AGSListItem<TItem>> getSingleItem() { yield return _singleItem; }
	}
}

