using System.Collections.Generic;

namespace AGS.API
{
    public interface IGameState
	{
		IPlayer Player { get; set; }
		IList<IRoom> Rooms { get; }
		IConcurrentHashSet<IObject> UI { get; }

		ICustomProperties GlobalVariables { get; }
		ICutscene Cutscene { get; }

		bool Paused { get; set; }
		int Speed { get; set; }

		void Clean();
		void CopyFrom(IGameState state);

		TObject Find<TObject>(string id) where TObject : class, IObject;
	}
}

