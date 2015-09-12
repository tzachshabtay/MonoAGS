using System;
using System.Collections.Generic;

namespace API
{
	public interface IInventory
	{
		IInventoryItem ActiveItem { get; set; }

		IEnumerable<IInventoryItem> Items { get; }

		IEvent<InventoryCombinationEventArgs> OnCombination { get; }
	}
}

