using System;
using System.Collections.Generic;

namespace AGS.API
{
    /// <summary>
    /// A binding list is a list which notifies on each list change
    /// </summary>
	public interface IAGSBindingList<TItem> : IList<TItem>
	{
        /// <summary>
        /// The event which is triggered on every list change (whenever an item is added from the list or removed from the list)
        /// </summary>
        /// <value>The event.</value>
		IEvent<AGSListChangedEventArgs<TItem>> OnListChanged { get; }
	}
}

