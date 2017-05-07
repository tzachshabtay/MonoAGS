namespace AGS.API
{
    /// <summary>
    /// A top-level interface to interact with the game.
    /// </summary>
    public interface IGame
	{
        /// <summary>
        /// Gets the factory for loading/creating all of the game objects.
        /// </summary>
        /// <value>The factory.</value>
		IGameFactory Factory { get; }

        /// <summary>
        /// Gets the game's state (rooms, objects, etc).
        /// </summary>
        /// <value>The state.</value>
		IGameState State { get; }

        /// <summary>
        /// Gets the game loop. This is used by the engine for updating the state every tick, and should
        /// not be used by the player.
        /// </summary>
        /// <value>The game loop.</value>
		IGameLoop GameLoop { get; }

        /// <summary>
        /// Interface for saving/loading the game.
        /// </summary>
        /// <value>The save load.</value>
		ISaveLoad SaveLoad { get; }

        /// <summary>
        /// Game's input (mouse, keyboard, etc).
        /// </summary>
        /// <value>The input.</value>
		IInput Input { get; }

        /// <summary>
        /// Gets the audio settings.
        /// </summary>
        /// <value>The audio settings.</value>
		IAudioSettings AudioSettings { get; }

        /// <summary>
        /// Gets the top-level game events (game loaded, on repeatedly execute, etc).
        /// </summary>
        /// <value>The events.</value>
		IGameEvents Events { get; }

        /// <summary>
        /// Gets the game's settings.
        /// </summary>
        /// <value>The settings.</value>
        IRuntimeSettings Settings { get; }

        /// <summary>
        /// Starts the game.
        /// </summary>
        /// <param name="settings">Settings.</param>
		void Start(IGameSettings settings);

        /// <summary>
        /// Quits the game. Note that this should not be used on IOS as IOS guidelines do not allow a button for quitting the game.
        /// </summary>
		void Quit();

        /// <summary>
        /// Find the entity with the specified id and type.
        /// </summary>
        /// <returns>The entity if found, null if no entity exists with that id.</returns>
        /// <param name="id">The entity's unique identifier.</param>
        /// <typeparam name="TEntity">The entity's type.</typeparam>
		TEntity Find<TEntity>(string id) where TEntity : class, IEntity;
	}
}

