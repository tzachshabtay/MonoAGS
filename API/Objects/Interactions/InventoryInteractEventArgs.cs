using System;

namespace API
{
	public class InventoryInteractEventArgs : ObjectEventArgs
	{
		public InventoryInteractEventArgs (IObject obj, IInventoryItem item) : base(obj)
		{
			Item = item;
		}

		public IInventoryItem Item { get; private set; }

		public override string ToString ()
		{
			return string.Format ("{0} interacted with {1}", base.ToString(), 
				Item == null ? "null" : Item.ToString());
		}
	}
}

