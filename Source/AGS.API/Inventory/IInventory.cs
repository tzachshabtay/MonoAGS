namespace AGS.API
{
    /// <summary>
    /// The character's inventory. Those are the items that the character holds in his/her (usually) imaginary bag.
    /// It is composed of a list of inventory items, and one active item that the character holds in his/her hand.
    /// </summary>
    public interface IInventory
	{
        /// <summary>
        /// Gets/sets the character's current active inventory item (for example the item that character currently holds in his/her hand). Setting it will update the mouse cursor if appropriate.
        /// To deselect the current inventory, set it to null.
        /// </summary>
        /// <value>
        /// The active inventory item.
        /// </value>
        /// <example>
        /// <code language="lang-csharp">
        /// cEgo.ActiveInventory = iKey; //The character is now "holding" the key.
        /// </code>
        /// <code language="lang-csharp">
        /// private async void onDoorClicked()
        /// {
        ///     if (cEgo.ActiveInventory == iKey) openDoor();
        ///     else await cEgo.SayAsync("I can't open the door without holding the key!");
        /// }
        /// </code>
        /// </example>
        IInventoryItem ActiveItem { get; set; }

        /// <summary>
        /// Gets the list of inventory items the character has.
        /// </summary>
        /// <example>
        /// Do I have the key?
        /// <code language="lang-csharp">
        /// if (Items.Any(item => item == iKey)) await cEgo.SayAsync("I have the key!");
        /// else await cEgo.SayAsync("I don't have the key!");
        /// </code>
        /// </example>
        /// <value>
        /// The inventory items.
        /// </value>
        IAGSBindingList<IInventoryItem> Items { get; }
	}
}

