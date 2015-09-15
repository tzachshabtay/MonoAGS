using System;
using System.Collections.Generic;

namespace AGS.API
{
	public interface IChildrenCollection : IEnumerable<IObject>
	{
		int Count { get; }
		void AddChild(IObject child);
		void RemoveChild(IObject child);
		bool HasChild(IObject child);
	}
}

