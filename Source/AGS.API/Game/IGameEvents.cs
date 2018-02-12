namespace AGS.API
{
    /// <summary>
    /// The game events, allows you to subscribe to important events that happen in the game.
    /// </summary>
    public interface IGameEvents
	{
        /// <summary>
        /// The on load event happens once in the game, it its startup, when it is loading.
        /// This is where all the rooms, objects and events are setup.
        /// </summary>
        /// <value>
        /// The on load event.
        /// </value>
        IBlockingEvent OnLoad { get; }

        /// <summary>
        /// The on repeatedly execute event happens every tick and allows you to check for conditions 
        /// and do specific actions when the conditions fulfill.
        /// The frequency of this events depends on the FPS (frames per second). By default, if the hardware (and software) can handle it,
        /// we run at 60 FPS, meaning this event will be called 60 times per second.
        /// 
        /// IMPORTANT: As this event runs 60 times per second (by default), it can be abused and deteriorate the performance of the game.
        /// So it's important to keep two rules:
        /// 1. Don't perform long actions on each tick.
        /// 2. Don't allocate memory on each tick.
        /// </summary>
        /// <value>
        /// The on repeatedly execute event.
        /// </value>
        /// <example>
        /// Let's look at some bad and good examples on how to use the event:
        /// <code language="lang-csharp">
        /// private void onRepeatedlyExecute()
        /// {        
        ///     //BAD!! running the dance animation on every tick will make the game run incredibly slow! 
        ///     performDanceAnimation(); 
        ///     
        ///     //BAD!! It's better than the previous line, but still, if the IsDancing will return true, we will be running the dance animation on every tick!
        ///     if (cEgo.IsDancing()) performDanceAnimation(); 
        ///     
        ///     //GOOD! We only running the dance animation once, not every tick, so we should be ok.
        ///     if (cEgo.IsDancing() &amp;&amp; Repeat.OnceOnly("DanceAnimation!!")) performDanceAnimation(); 
        /// 
        ///     //BAD!! We're allocating memory on every tick (the memory that we're allocating is the class we created called CheckShouldRunDance).
        ///     CheckShouldRunDance shouldRunDance = new CheckShouldRunDance();
        ///     if (shouldRunDance.ShouldDance() &amp;&amp; Repeat.OnceOnly("DanecAnimation!!")) performDanceAnimation();
        /// 
        ///     //GOOD!! We allocated this class outside the method once, and now we can use it (note the underscore prefix is just a convention used for class variables)...
        ///     if (_shouldRunDance.ShouldDance() &amp;&amp; Repeat.OnceOnly("DanceAnimation!!")) performDanceAnimation();
        /// }
        /// </code>        
        /// </example>
        IEvent OnRepeatedlyExecute { get; }

		/// <summary>
		/// This event is called on every render cycle before rendering starts.
		/// It can be used for native background drawings, or for native OpenGL calls
		/// for setting things up (like setting shader variables).
		/// </summary>
		/// <value>The on before render event.</value>
		IBlockingEvent OnBeforeRender { get; }

		/// <summary>
		/// This event is called whenever the screen is resized.
		/// </summary>
		/// <value>The on screen resize.</value>
		IBlockingEvent OnScreenResize { get; }

        /// <summary>
        /// The on saved game load is called whenever a saved game was loaded.
        /// This event can be used to rewire external code back to the game, and to make changes if needed when loading saves from previous versions of the game.
        /// </summary>
        /// <value>
        /// The on saved game load event.
        /// </value>
        /// <example>
        /// This examples show how we rewire external code back to the saved game.
        /// Let's say we saved the player to an external variable which is used outside the game state:
        /// <code language="lang-csharp">
        /// IPlayer _player;
        /// 
        /// public void LoadModule(IGame game)
        /// {
        ///     _player = game.State.Player;
        /// }
        /// </code>
        /// 
        /// All is nice and well, but if we load a saved game, _player will point to the player from before the save.
        /// We want to "rewire" it back to hold the correct player after the save, so we can rewrite the code like this:
        /// <code language="lang-csharp">
        /// private IPlayer _player;
        /// private IGame _game;
        /// 
        /// public void LoadModule(IGame game)
        /// {
        ///     _game = game;
        ///     _player = game.State.Player;
        ///     game.Events.OnSavedGameLoad.Subscribe(onSavedGameLoad);
        /// }
        /// 
        /// public void DisposeModule()
        /// {
        ///     //Whenever we subscribe to an event we need to remember to unsubscribe when we don't need it anymore, to avoid memory leaks.
        ///     game.Events.OnSavedGameLoad.Unsubscribe(onSavedGameLoad);
        /// }
        /// 
        /// private void onSavedGameLoad()
        /// {
        ///     _player = game.State.Player; //We rewire the _player variable to refer to the correct player!
        /// }
        /// </code>
        /// </example>
        /// <example>
        /// Let's look at an example of loading an incompatible save (a save from a previous version of the game).
        /// Let's say that we added a new version of our game with a new inventory item: a bowling ball.
        /// That bowling ball did not exist in the previous version of the game, but since we added a new puzzle it is now needed.
        /// But the player might saved a game after the last point he/she could have acquired the bowling ball.
        /// We, as game designers, might decide, that if we have the candle, then it's not possible for us not to have the bowling ball.
        /// So we can code it like this:
        /// <code language="lang-csharp">        
        /// private IGame _game;
        /// 
        /// public void LoadModule(IGame game)
        /// {
        ///     _game = game;
        ///     game.Events.OnSavedGameLoad.Subscribe(onSavedGameLoad);
        /// }
        /// 
        /// public void DisposeModule()
        /// {
        ///     //Whenever we subscribe to an event we need to remember to unsubscribe when we don't need it anymore, to avoid memory leaks.
        ///     _game.Events.OnSavedGameLoad.Unsubscribe(onSavedGameLoad);
        /// }
        /// 
        /// private void onSavedGameLoad()
        /// {
        ///     var inventory = _game.State.Player.Character.Inventory;
        ///     if (inventory.Contains(iCandle) &amp;&amp; !inventory.Contains(iBowlingBall))
        ///     {
        ///         inventory.Add(iBowlingBall);
        ///     }
        /// }
        /// </code>
        /// </example>
        IBlockingEvent OnSavedGameLoad { get; }

        /// <summary>
        /// Defines the default interactions for objects on the screen that can be interacted in some way, but for which we haven't
        /// defined specific interactions.
        /// </summary>
        /// <value>
        /// The default interactions.
        /// </value>
        IInteractions DefaultInteractions { get; }

        /// <summary>
        /// This event is fired whenever the visible room of the game is changing.
        /// The event is fired after the current room's fade-out event, and before the new room's fade-in event 
        /// (and after the viewport for the new room was updated).
        /// </summary>
        /// <value>The on room changing.</value>
        IBlockingEvent OnRoomChanging { get; }
	}
}

