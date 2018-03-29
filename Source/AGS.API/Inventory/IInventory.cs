using System.Collections.Generic;

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
        /// private void onDoorClicked()
        /// {
        ///     if (cEgo.ActiveInventory == iKey) openDoor();
        ///     else cEgo.Say("I can't open the door without holding the key!");
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
        /// if (Items.Any(item => item == iKey)) cEgo.Say("I have the key!");
        /// else cEgo.Say("I don't have the key!");
        /// </code>
        /// </example>
        /// <value>
        /// The inventory items.
        /// </value>
        IAGSBindingList<IInventoryItem> Items { get; }

        /// <summary>
        /// Called when one inventory item is combined with another.        
        /// </summary>
        /// <param name="item1">The "active" inventory item (the item that the character is currently "holding").</param>
        /// <param name="item2">The "passive" inventory item (the item we're using the "active" item on).</param>
        /// <returns>The event to subscribe.</returns>
        /// <example>
        /// First, we subscribe to the event:
        /// <code language="lang-csharp">
        /// cEgo.Inventory.OnCombination(iScissors, iRope).Subscribe(cutRope);
        /// cEgo.Inventory.OnCombination(iRope, iScissors).Subscribe(cutRope); //Assuming we want a symmetrical combination, we don't care which item is used on which
        /// </code>
        /// Then, we define the event (we describe what happens when using the scissors and the rope together:
        /// <code language="lang-csharp">
        /// private void cutRope(object sender, InventoryCombinationEventArgs args)
        /// {
        ///     displayCutRopeAnimation();
        ///     cEgo.Inventory.Items.Remove(iRope); //No more rope for us!
        /// }
        /// </code>
        /// </example>
        IEvent<InventoryCombinationEventArgs> OnCombination(IInventoryItem item1, IInventoryItem item2);

        /// <summary>
        /// Called when two inventory items are combined when there is no combination event subscribed specifically for them.
        /// </summary>
        /// <value>
        /// The default combination event.
        /// </value>
        /// <example>
        /// First, we subscribe to the event:
        /// <code language="lang-csharp">
        /// cEgo.Inventory.OnDefaultCombination.Subscribe(onDefaultCombination);
        /// </code>
        /// Then, we define the event (what do we do when the user tried to combine two random items?):
        /// <code language="lang-csharp">
        /// private void onDefaultCombination(object sender, InventoryCombinationEventArgs args)
        /// {
        ///     cEgo.Say(string.Format("I can't use {0} on {1}. It doesn't make sense!", args.ActiveItem.Graphics.Hotspot, args.PassiveItem.Graphics.Hotspot));
        /// }
        /// </code>
        /// </example>
        IEvent<InventoryCombinationEventArgs> OnDefaultCombination { get; }
	}
}

