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
        /// Gets the render loop. This is used by the engine for rendering to the screen on each tick, and should
        /// not be used by the player.
        /// </summary>
        /// <value>The renderer loop.</value>
        IRendererLoop RenderLoop { get; }

        /// <summary>
        /// Gets the render pipeline, which is responsible of collecting drawing instructions from all of the renderers
        /// and passing it on to the render loop.
        /// </summary>
        /// <value>The render pipeline.</value>
        IRenderPipeline RenderPipeline { get; }

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
        /// Game's audio system (the entry point for interacting with everything sound related).
        /// </summary>
        /// <value>The audio.</value>
        IAudioSystem Audio { get; }

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
        /// Gets the object under the mouse's position.
        /// </summary>
        /// <value>The hit test.</value>
        IHitTest HitTest { get; }

        /// <summary>
        /// Utility methods to help convert between different coordinate systems.
        /// </summary>
        /// <value>The coordinates.</value>
        ICoordinates Coordinates { get; }

        /// <summary>
        /// Allows retrieving various systems from the engine.
        /// </summary>
        /// <value>The resolver.</value>
        IResolver Resolver { get; }

        /// <summary>
        /// Starts the game.
        /// </summary>
		void Start();

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