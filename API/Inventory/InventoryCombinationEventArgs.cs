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

		public IInventoryItem ActiveItem { get; private set; }
		public IInventoryItem PassiveItem { get; private set; }

		public override string ToString ()
		{
			return string.Format ("{0} is combined with {1}", ActiveItem, PassiveItem);
		}
	}
}

