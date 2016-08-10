using System;
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

		public void ChangeRoom(IRoom newRoom, float? x = null, float? y = null)
		{
			if (Room != null)
			{
				if (_state.Player.Character == _obj)
				{
					Room.Events.OnBeforeFadeOut.Invoke(this, new AGSEventArgs ());
					_roomTransitions.State = RoomTransitionState.BeforeLeavingRoom;
					if (_roomTransitions.Transition != null)
						_roomTransitions.OnStateChanged.WaitUntil(_ => _roomTransitions.State == RoomTransitionState.PreparingTransition
							|| _roomTransitions.State == RoomTransitionState.NotInTransition);
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

