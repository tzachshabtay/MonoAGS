using AGS.API;

namespace AGS.Engine
{
    public interface IAGSGameState : IGameState
    {
        IEvent<RoomTransitionEventArgs> OnRoomChangeRequired { get; }
    }
}