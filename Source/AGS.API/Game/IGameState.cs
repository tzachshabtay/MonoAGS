using System;

namespace AGS.API
{
    public interface IGameState
	{
		IPlayer Player { get; set; }
        IRoom Room { get; }
		IAGSBindingList<IRoom> Rooms { get; }
		IConcurrentHashSet<IObject> UI { get; }

		ICustomProperties GlobalVariables { get; }
		ICutscene Cutscene { get; }

		IRoomTransitions RoomTransitions { get; }

		bool Paused { get; set; }
		int Speed { get; set; }

        void ChangeRoom(IRoom newRoom, Action afterTransitionFadeOut = null);
		void Clean();
		void CopyFrom(IGameState state);

		TEntity Find<TEntity>(string id) where TEntity : class, IEntity;
	}
}

