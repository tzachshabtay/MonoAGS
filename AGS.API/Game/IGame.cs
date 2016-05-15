namespace AGS.API
{
    public interface IGame
	{
		IGameFactory Factory { get; }
		IGameState State { get; }

		IGameLoop GameLoop { get; }
		ISaveLoad SaveLoad { get; }

		IInput Input { get; }

		IGameEvents Events { get; }

		Size VirtualResolution { get; }
		Size WindowSize { get; }

		void Start(IGameSettings settings);
		void Quit();

		TEntity Find<TEntity>(string id) where TEntity : class, IEntity;
	}
}

