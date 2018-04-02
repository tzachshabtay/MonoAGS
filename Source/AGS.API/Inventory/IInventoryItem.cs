namespace AGS.API
{
    /// <summary>
    /// Represents an inventory item that a character can hold in his/her inventory.
    /// </summary>
    public interface IInventoryItem
	{
        /// <summary>
        /// Gets or sets the graphics that represents the inventory item when it sits in the inventory window.
        /// The graphics can be a single image or an animation.
        /// </summary>
        /// <value>
        /// The graphics.
        /// </value>
        IObject Graphics { get; set; }
        /// <summary>
        /// Gets or sets the graphics that represents the inventory item when the mouse cursor is to be shaped like the inventory item
        /// (i.e when the character holds the inventory and wants to use it on something).
        /// The graphics can be a single image or an animation.
        /// This is only relevant when using a mouse cursor (in touch platforms it won't be used, unless we're simulating a mouse).
        /// </summary>
        /// <value>
        /// The cursor graphics.
        /// </value>
        IObject CursorGraphics { get; set; }
        /// <summary>
        /// Gets or sets the quantity of the item of the inventory.
        /// Most inventory items don't have quantity, they're either there or not.
        /// But, if the inventory item is money, for example, you can use the "Qty" variable to
        /// indicate how much money you currently have in your inventory.
        /// </summary>
        /// <value>
        /// The item quantity.
        /// </value>
        float Qty { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether this inventory item is an inventory item that you can only interact with but not 
        /// combine with other items or objects in the room.
        /// Most items will have this as false, but it might be useful in rare cases.
        /// For example, if you have a trumpet, and you want the character to be able to play the trumpet at any given time, but don't actually
        /// want to use the trumpet on other items, you can assign ShouldInteract = true, and subscribe to the interact event, in order to play the music.
        /// </summary>
        /// <value>
        ///   <c>true</c> if should use the interact events; otherwise, it will combine as normal.
        /// </value>
        bool ShouldInteract { get; set; }

        /// <summary>
        /// Called when one inventory item is combined with another.
        /// <seealso cref="IDefaultInteractions.OnInventoryCombination"/>
        /// </summary>
        /// <param name="otherItem">The second inventory item, which is combined with this inventory item.</param>
        /// <returns>The event to subscribe.</returns>
        /// <example>
        /// First, we subscribe to the event:
        /// <code language="lang-csharp">
        /// iScissors.OnCombination(iRope).Subscribe(cutRope); //The "cutRope" will be called whether the scissors were used on the rope, or vice versa.
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
        IEvent<InventoryCombinationEventArgs> OnCombination(IInventoryItem otherItem);
	}
}

