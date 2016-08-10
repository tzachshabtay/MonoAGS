namespace AGS.API
{
    /// <summary>
    /// Defines the default interactions for objects on the screen that can be interacted in some way, but for which we haven't
    /// defined specific interactions.
    /// </summary>
    public interface IInteractions
	{
        /// <summary>
        /// The default event for when looking on an object without a specific look event.
        /// </summary>
        /// <value>
        /// The on default look event.
        /// </value>
        /// <example>
        /// <code>
        /// IGame _game;
        /// 
        /// public void LoadModule(IGame game)
        /// {
        ///     _game = game;        
        ///     _game.Events.DefaultInteractions.OnLook.Subscribe(onDefaultLook);
        /// }
        /// 
        /// public void DisposeModule()
        /// {
        ///     //Whenever we subscribe to an event we need to remember to unsubscribe when we don't need it anymore, to avoid memory leaks.
        ///     _game.Events.DefaultInteractions.OnLook.Unsubscribe(onDefaultLook);
        /// }
        /// 
        /// private void onDefaultLook(object sender, ObjectEventArgs args)
        /// {
        ///     cEgo.Say(string.Format("What am I looking at? Oh, it's {0}. It looks nice, I guess."), args.Object.Hotspot);
        /// }
        /// </code>
        /// </example>
        IEvent<ObjectEventArgs> OnLook { get; }
        /// <summary>
        /// The default event for when interacting with an object without a specific interact event.
        /// </summary>
        /// <value>
        /// The on default interact event.
        /// </value>
        /// <example>
        /// <code>
        /// IGame _game;
        /// 
        /// public void LoadModule(IGame game)
        /// {
        ///     _game = game;        
        ///     _game.Events.DefaultInteractions.OnInteract.Subscribe(onDefaultInteract);
        /// }
        /// 
        /// public void DisposeModule()
        /// {
        ///     //Whenever we subscribe to an event we need to remember to unsubscribe when we don't need it anymore, to avoid memory leaks.
        ///     _game.Events.DefaultInteractions.OnInteract.Unsubscribe(onDefaultInteract);
        /// }
        /// 
        /// private void onDefaultInteract(object sender, ObjectEventArgs args)
        /// {
        ///     cEgo.Say("{0}? No, I'm not touching it.", args.Object.Hotspot);
        /// }
        /// </code>
        /// </example>
		IEvent<ObjectEventArgs> OnInteract { get; }
        /// <summary>
        /// The default event for when using an inventory item on an object without a specific inventory interact event for that object.
        /// </summary>
        /// <value>
        /// The on default inventory interact event.
        /// </value>
        /// <example>
        /// <code>
        /// IGame _game;
        /// 
        /// public void LoadModule(IGame game)
        /// {
        ///     _game = game;        
        ///     _game.Events.DefaultInteractions.OnInteract.Subscribe(onDefaultInventoryInteract);
        /// }
        /// 
        /// public void DisposeModule()
        /// {
        ///     //Whenever we subscribe to an event we need to remember to unsubscribe when we don't need it anymore, to avoid memory leaks.
        ///     _game.Events.DefaultInteractions.OnInteract.Unsubscribe(onDefaultInventoryInteract);
        /// }
        /// 
        /// private void onDefaultInventoryInteract(object sender, InventoryInteractEventArgs args)
        /// {
        ///     cEgo.Say("Using {0} on {1}? No, I don't think so.", args.Item.Graphics.Hotspot, args.Object.Hotspot);
        /// }
        /// </code>
        /// </example>
        IEvent<InventoryInteractEventArgs> OnInventoryInteract { get; }
        /// <summary>
        /// The default event for when custom interacting with an object without a specific custom interact event.
        /// The possible custom interactions depend on your control scheme. For example, you might have a separate icon for talk
        /// in your game which is different from the interact icon.
        /// </summary>
        /// <value>
        /// The on default interact event.
        /// </value>
        /// <example>
        /// <code>
        /// IGame _game;
        /// 
        /// public void LoadModule(IGame game)
        /// {
        ///     _game = game;        
        ///     _game.Events.DefaultInteractions.OnCustomInteract.Subscribe(onCustomInteract);
        /// }
        /// 
        /// public void DisposeModule()
        /// {
        ///     //Whenever we subscribe to an event we need to remember to unsubscribe when we don't need it anymore, to avoid memory leaks.
        ///     _game.Events.DefaultInteractions.OnCustomInteract.Unsubscribe(onCustomInteract);
        /// }
        /// 
        /// private void onCustomInteract(object sender, CustomInteractionEventArgs args)
        /// {
        ///     if (args.InteractionName == "Talk")
        ///     {
        ///         cEgo.Say("I don't want to speak with {0}.", args.Object.Hotspot);
        ///     } 
        ///     else
        ///     {
        ///         cEgo.Say("I don't want any '{0}ing' with {1}.", args.InteractionName, args.Object.Hotspot);
        ///     }  
        /// }
        /// </code>
        /// </example>
		IEvent<CustomInteractionEventArgs> OnCustomInteract { get; }
	}
}

