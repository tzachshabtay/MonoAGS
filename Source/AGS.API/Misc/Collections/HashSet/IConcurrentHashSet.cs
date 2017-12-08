using System;
using System.Collections.Generic;

namespace AGS.API
{
    /// <summary>
    /// A hashed set which can be write/read concurrently.
    /// </summary>
	public interface IConcurrentHashSet<TItem> : IEnumerable<TItem>
	{
        /// <summary>
        /// Gets the number of items in the set.
        /// </summary>
        /// <value>The count.</value>
		int Count { get; }

        /// <summary>
        /// An event which is triggered whenever the set is changed (an item is added or removed).
        /// </summary>
        /// <value>The event.</value>
        IBlockingEvent<AGSHashSetChangedEventArgs<TItem>> OnListChanged { get; }

        /// <summary>
        /// Add the specified item.
        /// </summary>
        /// <returns>True if item was added, false if it was already in the set.</returns>
        /// <param name="item">Item.</param>
		bool Add(TItem item);

        /// <summary>
        /// Adds the specified items.
        /// </summary>
        /// <returns>The number of items that was actually added (were not already in the set).</returns>
        /// <param name="items">Items.</param>
        int AddRange(List<TItem> items);

        /// <summary>
        /// Remove the specified item.
        /// </summary>
        /// <returns>True if item was removed, false if it was not in the set.</returns>
        /// <param name="item">Item.</param>
		bool Remove(TItem item);

        /// <summary>
        /// Removes all items that match the predicate from the set.
        /// </summary>
        /// <param name="shouldRemove">The predicate to decide which item should be removed.</param>
		void RemoveAll(Predicate<TItem> shouldRemove);

        /// <summary>
        /// Is the specified item in the set?
        /// </summary>
        /// <returns>True if the item is in the set, false if it isn't.</returns>
        /// <param name="item">Item.</param>
		bool Contains(TItem item);

        /// <summary>
        /// Clears the set.
        /// </summary>
		void Clear();
	}
}

