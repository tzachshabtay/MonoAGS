using System.Collections.Generic;

namespace AGS.API
{
    public interface IInventory
	{
		IInventoryItem ActiveItem { get; set; }

		IList<IInventoryItem> Items { get; }

		IEvent<InventoryCombinationEventArgs> OnCombination(IInventoryItem item1, IInventoryItem item2);

		IEvent<InventoryCombinationEventArgs> OnDefaultCombination { get; }
	}
}

