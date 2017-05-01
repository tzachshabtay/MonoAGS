namespace AGS.API
{
    /// <summary>
    /// A pre-set entity with all of the UI control components, plus an inventory window component, 
    /// for displaying inventory items in a window.
    /// </summary>
    /// <seealso cref="IInventory"/>
    /// <seealso cref="IInventoryItem"/>
	public interface IInventoryWindow : IUIControl, IInventoryWindowComponent
	{
	}    
}

