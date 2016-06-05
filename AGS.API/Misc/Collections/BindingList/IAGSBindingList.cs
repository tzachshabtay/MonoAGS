using System;
using System.Collections.Generic;

namespace AGS.API
{
	public interface IAGSBindingList<TItem> : IList<TItem>
	{
		IEvent<AGSListChangedEventArgs<TItem>> OnListChanged { get; }
	}
}

