namespace AGS.API
{
    /// <summary>
    /// The event arguments for the inventory combination events.
    /// The arguments contain the two combined items.
    /// </summary>
    public class InventoryCombinationEventArgs
	{
        /// <summary>
        /// Initializes a new instance of the <see cref="T:AGS.API.InventoryCombinationEventArgs"/> class.
        /// </summary>
        /// <param name="activeItem">Active item.</param>
        /// <param name="passiveItem">Passive item.</param>
		public InventoryCombinationEventArgs (IInventoryItem activeItem, IInventoryItem passiveItem)
		{
			ActiveItem = activeItem;
			PassiveItem = passiveItem;
		}

        /// <summary>
        /// Gets the active item for the combination (the item that you are using on the other item).
        /// </summary>
        /// <value>
        /// The active item.
        /// </value>
        public IInventoryItem ActiveItem { get; private set; }
        /// <summary>
        /// Gets the passive item for the combination (the item that you use the other item on).
        /// </summary>
        /// <value>
        /// The passive item.
        /// </value>
        public IInventoryItem PassiveItem { get; private set; }

		public override string ToString ()
		{
			return $"{ActiveItem} is combined with {PassiveItem}";
		}
	}
}

