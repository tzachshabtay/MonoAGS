using System;
using System.Threading.Tasks;
using AGS.API;

namespace AGS.Engine
{
	public class HasRoomBehavior : AGSComponent, IHasRoom
	{
		private IObject _obj;
		private readonly IGameState _state;
		private Lazy<IRoom> _cachedRoom;
		private IAGSRoomTransitions _roomTransitions;

		public HasRoomBehavior(IGameState state, IAGSRoomTransitions roomTransitions)
		{
			_state = state;
			_roomTransitions = roomTransitions;
			refreshRoom();
		}

		public override void Init(IEntity entity)
		{
			base.Init(entity);
			_obj = (IObject)entity;
		}

		public IRoom Room  { get { return _cachedRoom.Value; } }

		public IRoom PreviousRoom { get; private set; }

        public async Task ChangeRoomAsync(IRoom newRoom, float? x = null, float? y = null)
		{
            bool firstRoom = PreviousRoom == null;
            Action changeRoom = () => 
            {
                var room = Room;
                if (room != null) room.Objects.Remove(_obj);
                if (x != null) _obj.X = x.Value;
                if (y != null) _obj.Y = y.Value;
                if (newRoom != null)
                {
                    newRoom.Objects.Add(_obj);
                }
                PreviousRoom = Room;
                refreshRoom();
            };
            if (_state.Player == _obj) await _state.ChangeRoomAsync(newRoom, changeRoom);
            else changeRoom();
			
            //Waiting for a transition state change to ensure the before fade in event of the new room occurs before the next action after the ChangeRoom was called
            if (_state.Player == _obj && !firstRoom && _roomTransitions.Transition != null)
                await _roomTransitions.OnStateChanged.WaitUntilAsync(canCompleteRoomTransition);
		}

        private bool canCompleteRoomTransition(object args)
        {
            return _roomTransitions.State == RoomTransitionState.NotInTransition;
        }

        private void refreshRoom()
		{
			_cachedRoom = new Lazy<IRoom> (getRoom, true);
		}

		private IRoom getRoom()
		{
			if (_state == null) 
				return null;
			foreach (var room in _state.Rooms)
			{
				if (room.Objects.Contains(_obj)) return room;
			}
			return null;
		}
	}
}

