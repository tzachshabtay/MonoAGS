namespace AGS.API
{
    /// <summary>
    /// Default interactions (when no specific interaction callback is subscribed) are defined here.
    /// This is where you define generic responses (like the famous "this doesn't work") that cover all of the interactions for which 
    /// you didn't code a specific response.
    /// </summary>
    public interface IDefaultInteractions : IInteractions
    {
        /// <summary>
        /// Called when two inventory items are combined and there is no combination event subscribed specifically for them.
        /// <seealso cref="IInventoryItem.OnCombination"/>
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
        ///     cEgo.Say($"I can't use {args.ActiveItem.Graphics.DisplayName} on {args.PassiveItem.Graphics.DisplayName}. It doesn't make sense!");
        /// }
        /// </code>
        /// </example>
        IEvent<InventoryCombinationEventArgs> OnInventoryCombination { get; }
    }
}