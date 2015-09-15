using System;

namespace AGS.API
{
	public interface IInventoryItem
	{
		string ID { get; }
		string DisplayName { get; }
		int Qty { get; }
	}
}

