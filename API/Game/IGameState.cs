using System;
using System.Collections.Generic;
using System.Threading.Tasks;

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

		void CopyFrom(IGameState state);

		TObject Find<TObject>(string id) where TObject : class, IObject;
	}
}

