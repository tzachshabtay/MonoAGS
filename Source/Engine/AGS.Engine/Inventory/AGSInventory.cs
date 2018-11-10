using AGS.API;

namespace AGS.Engine
{
    [PropertyFolder]
    [ConcreteImplementation(DisplayName = "Inventory")]
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

        public override string ToString() => $"{Items.Count} inventory item(s)";
    }
}