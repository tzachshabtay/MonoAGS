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
        private Task[] _emptyTaskArray = new Task[] { };

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

        public void ChangeRoom(IRoom newRoom, float? x = null, float? y = null)
		{
			if (Room != null)
			{
				if (_state.Player.Character == _obj)
				{
                    if (_roomTransitions.State != RoomTransitionState.NotInTransition) //Room is already changing, need to wait for previous transition to complete before starting a new transition!
                    {
                        while (_roomTransitions.State != RoomTransitionState.NotInTransition)
                        {
                            Task.WaitAll(_emptyTaskArray,10); //Busy waiting, agghh!
                        }
                        if (Room == newRoom) return; //somebody already changed to this room, no need to change again.
                    }
					Room.Events.OnBeforeFadeOut.Invoke(this, new AGSEventArgs ());
					_roomTransitions.State = RoomTransitionState.BeforeLeavingRoom;
					if (_roomTransitions.Transition != null)
                        _roomTransitions.OnStateChanged.WaitUntil(canContinueRoomTransition);
				}
				Room.Objects.Remove(_obj);
			}
			if (newRoom != null)
			{
				newRoom.Objects.Add(_obj);
			}
			PreviousRoom = Room;
			refreshRoom();

			if (x != null) _obj.X = x.Value;
			if (y != null) _obj.Y = y.Value;
		}

        private bool canContinueRoomTransition(AGSEventArgs args)
        {
            return _roomTransitions.State == RoomTransitionState.PreparingTransition ||
                   _roomTransitions.State == RoomTransitionState.NotInTransition;
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

