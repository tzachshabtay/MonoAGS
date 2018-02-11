namespace AGS.API
{
    /// <summary>
    /// Allows subscribing interaction events (to define what happens when an object is looked at/interacted with/etc). 
    /// </summary>
    public interface IInteractions
	{
        /// <summary>
        /// The event for when interacting with an object.
        /// The available verbs depend on your control scheme. For example, you might have a separate icon for talk
        /// in your game which is different from the interact icon.
        /// There's also a "Default" event which will be used if no specific event was subscribed to.
        /// There's a chain of default behaviors that can be defined to allow for generic responses.
        /// The event handler lookup is as follows: 
        /// 1. Object's verb
        /// 2. Object's default verb
        /// 3. IGame.Events.DefaultInteractions verb
        /// 4. IGame.Events.DefaultInteractions default verb
        /// </summary>
        /// <value>
        /// The interaction event.
        /// </value>
        /// <example>
        /// <code language="lang-csharp">
        /// public void SubscribeEvents()
        /// {
        ///     game.Events.DefaultInteractions.OnInteract("Talk").Subscribe(onDefaultTalk);
        ///     oTeapot.Interactions.OnInteract(AGSInteractions.Look).Subscribe(onTeapotLook);
        ///     oTeapot.Interactions.OnInteract(AGSInteractions.Interact).SubscribeToAsync(onTeapotInteract);
        ///     oTeapot.Interactions.OnInteract("Throw").Subscribe(onTeapotThrow);
        ///     oTeapot.Interactions.OnInteract(AGSInteractions.Default).Subscribe(onTeapotDefault);
        /// }
        /// 
        /// private void onDefaultTalk(ObjectEventArgs args)
        /// {
        ///     cEgo.Say(string.Format("{0}? No, I don't think it's going to talk back.", args.Object.Hotspot));
        /// }
        /// 
        /// private void onTeapotLook(ObjectEventArgs args)
        /// {
        ///     cEgo.Say("What a lovely looking teapot!");
        /// }
        /// 
        /// private async Task onTeapotInteract(ObjectEventArgs args)
        /// {
        ///     await cEgo.SayAsync("I'm going to pour some tea now.");
        ///     oTeapot.StartAnimation(aPourTea);
        /// }
        /// 
        /// private void onTeapotThrow(ObjectEventArgs args)
        /// {
        ///     cEgo.Say("No way, I'm not throwing the teapot.");
        /// }
        /// 
        /// private void onTeapotDefault(ObjectEventArgs args)
        /// {
        ///     cEgo.Say("I'm not doing anything like that to the teapot.");
        /// }
        /// </code>
        /// </example>
        IEvent<ObjectEventArgs> OnInteract(string verb);

        /// <summary>
        /// The event for when using an inventory item on an object.
        /// The available verbs depend on your control scheme. For example, you might have a separate icon for give
        /// item in your game which is different from the interact icon.
        /// There's also a "Default" event which will be used if no specific event was subscribed to.
        /// There's a chain of default behaviors that can be defined to allow for generic responses.
        /// The event handler lookup is as follows: 
        /// 1. Object's verb
        /// 2. Object's default verb
        /// 3. IGame.Events.DefaultInteractions verb
        /// 4. IGame.Events.DefaultInteractions default verb
        /// </summary>
        /// <value>
        /// The inventory interaction event.
        /// </value>
        /// <example>
        /// <code language="lang-csharp">
        /// public void SubscribeEvents()
        /// {
        ///     oTeapot.Interactions.OnInventoryInteract(AGSInteractions.Interact).Subscribe(onTeapotInventoryInteract);
        ///     game.Events.DefaultInteractions.OnInventoryInteract(AGSInteractions.Interact).Subscribe(onDefaultInventoryInteract);
        /// }
        /// 
        /// private void onTeapotInventoryInteract(InventoryInteractEventArgs args)
        /// {
        ///     if (args.Item == iCup)
        ///     {
        ///         cEgo.Say("Ok, I'm going to pour tea in the cup");
        ///         oTeapot.StartAnimation(aPourTea);
        ///         cEgo.Inventory.Items.Remove(iCup);
        ///         cEgo.Inventory.Items.Add(iFullCup);
        ///     }
        ///     else if (args.Item == iFullCup)
        ///     {
        ///         cEgo.Say("The cup is already full.");
        ///     }
        ///     else
        ///     {
        ///         cEgo.Say("This is not what you'd usually use on a teapot...");
        ///     }
        /// }
        /// private void onDefaultInventoryInteract(InventoryInteractEventArgs args)
        /// {
        ///     cEgo.Say(string.Format("Using {0} on {1}? No, I don't think so.", args.Item.Graphics.Hotspot, args.Object.Hotspot));
        /// }
        /// </code>
        /// </example>
        IEvent<InventoryInteractEventArgs> OnInventoryInteract(string verb);
	}
}

