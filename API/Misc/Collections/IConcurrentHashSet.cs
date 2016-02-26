using System;
using System.Collections.Generic;

namespace AGS.API
{
	public interface IConcurrentHashSet<TItem> : IEnumerable<TItem>
	{
		int Count { get; }

		bool Add(TItem item);
		bool Remove(TItem item);
		void RemoveAll(Predicate<TItem> shouldRemove);
		bool Contains(TItem item);
		void Clear();
	}
}

