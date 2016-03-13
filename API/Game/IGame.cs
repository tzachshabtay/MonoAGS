using System.Drawing;

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

		void Start(IGameSettings settings);
		void Quit();

		TObject Find<TObject>(string id) where TObject : class, IObject;
	}
}

