using AGS.API;

namespace AGS.Engine
{
    [PropertyFolder]
	public class AGSInventoryItem : IInventoryItem
	{
        private readonly InventorySubscriptions _subscriptions;

        public AGSInventoryItem(InventorySubscriptions subscriptions)
		{
            _subscriptions = subscriptions;
			Qty = 1f;
		}

		#region IInventoryItem implementation

		public IObject Graphics { get; set; }

		public IObject CursorGraphics { get; set; }

		public float Qty { get; set; }

		public bool ShouldInteract { get; set; }

        public IEvent<InventoryCombinationEventArgs> OnCombination(IInventoryItem otherItem)
        {
            return _subscriptions.Subscribe(this, otherItem);
        }

        #endregion

        public override string ToString()
		{
			return $"Inventory Item: {Graphics.GetFriendlyName() ?? Graphics.ToString()}";
		}
	}
}