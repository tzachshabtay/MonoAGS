using System;

namespace AGS.API
{
	public class InventoryCombinationEventArgs : EventArgs
	{
		public InventoryCombinationEventArgs (IInventoryItem activeItem, IInventoryItem passiveItem)
		{
			ActiveItem = activeItem;
			PassiveItem = passiveItem;
		}

		IInventoryItem ActiveItem { get; }
		IInventoryItem PassiveItem { get; }

		public override string ToString ()
		{
			return string.Format ("{0} is combined with {1}", ActiveItem, PassiveItem);
		}
	}
}

