using AGS.API;

namespace AGS.Engine
{
    public class AGSSingleRoomProvider : IRoomProvider
    {
        public AGSSingleRoomProvider(IRoom room)
        {
            Room = room;
        }

        public IRoom Room { get; private set; }
    }
}
