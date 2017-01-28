using System;
using System.Threading.Tasks;

namespace AGS.API
{
    public interface IGameState
	{
        ICharacter Player { get; set; }
        IRoom Room { get; }
		IAGSBindingList<IRoom> Rooms { get; }
		IConcurrentHashSet<IObject> UI { get; }
        IFocusedUI FocusedUI { get; }

		ICustomProperties GlobalVariables { get; }
		ICutscene Cutscene { get; }

		IRoomTransitions RoomTransitions { get; }

		bool Paused { get; set; }
		int Speed { get; set; }

        Task ChangeRoomAsync(IRoom newRoom, Action afterTransitionFadeOut = null);
		void Clean();
		void CopyFrom(IGameState state);

		TEntity Find<TEntity>(string id) where TEntity : class, IEntity;
	}
}

