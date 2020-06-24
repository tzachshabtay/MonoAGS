﻿using System;
using System.Linq;
using System.Threading.Tasks;
using AGS.API;

namespace AGS.Engine
{
	public class RoomComponent : AGSComponent, IHasRoomComponent
	{
		private IObject _obj;
		private readonly IGameState _state;
		private Lazy<IRoom> _cachedRoom;

		public RoomComponent(IGameState state)
		{
			_state = state;
            OnRoomChanged = new AGSEvent();
			refreshRoom();
            state.Rooms?.OnListChanged?.Subscribe(onRoomsChanged);
		}

        public override void Init()
		{
			base.Init();
			_obj = (IObject)Entity;
		}

        public override void Dispose()
        {
            base.Dispose();
            _state?.Rooms?.OnListChanged?.Unsubscribe(onRoomsChanged);
            _cachedRoom = null;
            _obj = null;
        }

        [Property(DisplayName = "Current Room")]
        public IRoom Room => _cachedRoom?.Value;

        public IRoom PreviousRoom { get; private set; }

        public IBlockingEvent OnRoomChanged { get; }

        public async Task ChangeRoomAsync(IRoom newRoom, float? x = null, float? y = null)
		{
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
            return _state?.Rooms?.FirstOrDefault(r => r.Objects.Contains(_obj) || r.Background == _obj);
		}
	}
}
