using System;
using System.Collections.Generic;

namespace API
{
	public interface IConcurrentHashSet<TItem> : IEnumerable<TItem>
	{
		int Count { get; }

		bool Add(TItem item);
		bool Remove(TItem item);
		bool Contains(TItem item);
	}
}

