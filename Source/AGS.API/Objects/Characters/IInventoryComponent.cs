namespace AGS.API
{
    /// <summary>
    /// This component allows an entity to carry inventory.
    /// </summary>
    /// <seealso cref="IComponent"/>
	public interface IInventoryComponent : IComponent
	{
        /// <summary>
        /// Gets or sets the inventory which is held by the entity.
        /// </summary>
        /// <value>The inventory.</value>
		IInventory Inventory { get; set; }
	}
}

