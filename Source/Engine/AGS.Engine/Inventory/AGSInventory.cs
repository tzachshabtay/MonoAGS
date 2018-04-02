using AGS.API;

namespace AGS.Engine
{
    [PropertyFolder]
	public class AGSInventory : IInventory
	{
		public AGSInventory()
		{
            Items = new AGSBindingList<IInventoryItem> (20);
		}

		#region IInventory implementation

		public IInventoryItem ActiveItem { get; set; }

        public IAGSBindingList<IInventoryItem> Items { get; private set; }

		#endregion
	}
}