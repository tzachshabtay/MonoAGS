using System;
using System.Linq;
using System.Threading.Tasks;
using AGS.API;

namespace AGS.Engine
{
	public class HasRoomComponent : AGSComponent, IHasRoomComponent
	{
		private IObject _obj;
		private readonly IGameState _state;
		private Lazy<IRoom> _cachedRoom;
		private IAGSRoomTransitions _roomTransitions;

		public HasRoomComponent(IGameState state, IAGSRoomTransitions roomTransitions)
		{
			_state = state;
			_roomTransitions = roomTransitions;
            OnRoomChanged = new AGSEvent();
			refreshRoom();
            state.Rooms?.OnListChanged?.Subscribe(onRoomsChanged);
		}

        public override void Init(IEntity entity)
		{
			base.Init(entity);
			_obj = (IObject)entity;
		}

        public override void Dispose()
        {
            base.Dispose();
            _state?.Rooms?.OnListChanged?.Unsubscribe(onRoomsChanged);
            _cachedRoom = null;
            _obj = null;
        }

        public IRoom Room => _cachedRoom?.Value ?? null;

        public IRoom PreviousRoom { get; private set; }

        public IBlockingEvent OnRoomChanged { get; }

        public async Task ChangeRoomAsync(IRoom newRoom, float? x = null, float? y = null)
		{
            bool firstRoom = PreviousRoom == null;
            Action changeRoom = () => 
            {
                Room?.Objects.Remove(_obj);
                if (x != null) _obj.X = x.Value;
                if (y != null) _obj.Y = y.Value;
                newRoom?.Objects.Add(_obj);
                PreviousRoom = Room;
                refreshRoom();
            };
            if (_state.Player == _obj) await _state.ChangeRoomAsync(newRoom, changeRoom);
            else changeRoom();
			
            //Waiting for a transition state change to ensure the before fade in event of the new room occurs before the next action after the ChangeRoom was called
            if (_state.Player == _obj && !firstRoom && _roomTransitions.Transition != null)
                await _roomTransitions.OnStateChanged.WaitUntilAsync(canCompleteRoomTransition);
		}

        private bool canCompleteRoomTransition()
        {
            return _roomTransitions.State == RoomTransitionState.NotInTransition;
        }

        private void onRoomsChanged(AGSListChangedEventArgs<IRoom> args)
        {
            if (Room == null && args.ChangeType == ListChangeType.Add) refreshRoom();
        }

        private void refreshRoom()
		{
			_cachedRoom = new Lazy<IRoom> (getRoom, true);
            OnPropertyChanged(nameof(Room));
            OnRoomChanged.Invoke();
		}

		private IRoom getRoom()
		{
            return _state?.Rooms?.FirstOrDefault(r => r.Objects.Contains(_obj));
		}
	}
}
