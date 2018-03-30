using System;
using AGS.API;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Threading;

namespace AGS.Engine
{
	public class AGSGameLoop : IGameLoop
	{
		private readonly IGameState _gameState;
		private AGS.API.Size _virtualResolution;
		private IRoom _lastRoom;
		private readonly IAGSRoomTransitions _roomTransitions;
        private readonly IGameEvents _events;
        private int _inUpdate; //For preventing re-entrancy
        private readonly IDisplayList _displayList;

		public AGSGameLoop (IGameState gameState, AGS.API.Size virtualResolution, 
                            IAGSRoomTransitions roomTransitions, IGameEvents events, 
                            IDisplayList displayList)
		{
            _displayList = displayList;
			_gameState = gameState;
            _events = events;
			_virtualResolution = virtualResolution;
			_roomTransitions = roomTransitions;
		}

		#region IGameLoop implementation

		public void Update()
		{
            if (Interlocked.CompareExchange(ref _inUpdate, 1, 0) != 0) return;
            try
            {
                if (_gameState.Room == null) return;
                IRoom room = _gameState.Room;
                bool changedRoom = _lastRoom != room;
                if (_roomTransitions.State != RoomTransitionState.NotInTransition)
                {
                    if (_roomTransitions.State == RoomTransitionState.PreparingTransition)
                    {
                        if (changedRoom)
                        {
                            _lastRoom?.Events.OnAfterFadeOut.Invoke();
                            room.Events.OnBeforeFadeIn.Invoke();
                            updateViewports(changedRoom);
                            _events.OnRoomChanging.Invoke();
                            if (_lastRoom == null) _roomTransitions.State = RoomTransitionState.NotInTransition;
                            else
                            {
                                _displayList.Update();
                                updateViewports(changedRoom);
                                _roomTransitions.State = RoomTransitionState.PreparingNewRoomRendering;
                            }
                        }
                    }
                    return;
                }

                updateViewports(changedRoom);
                _displayList.Update();
                updateRoom(room);
            }
            finally
            {
                _inUpdate = 0;
            }
		}

		#endregion

        private void updateViewports(bool playerChangedRoom)
        {
            updateViewport(_gameState.Viewport, playerChangedRoom);
            try
            {
                foreach (var viewport in _gameState.SecondaryViewports)
                {
                    updateViewport(viewport, playerChangedRoom);
                }
            }
            catch (InvalidOperationException) { } //can be triggered if a viewport was added/removed while enumerating- this should be resolved on next tick
		}

        private void updateViewport (IViewport viewport, bool playerChangedRoom)
		{
            runAnimations(viewport.RoomProvider.Room);
            viewport.Camera?.Tick(viewport, viewport.RoomProvider.Room.Limits, _virtualResolution, playerChangedRoom);
		}

		private void runAnimations(IRoom room)
		{
            if (room == null) return;
			if (room.Background != null) runAnimation(room.Background.Animation);
			foreach (var obj in room.Objects)
			{
				if (!obj.Visible)
					continue;
				if (!room.ShowPlayer && obj == _gameState.Player)
					continue;
				runAnimation(obj.Animation);
			}
		}

		private void updateRoom(IRoom room)
		{
			if (_lastRoom == room) return;
            _lastRoom = room;
            room.Events.OnAfterFadeIn.InvokeAsync();
		}

		private void runAnimation(IAnimation animation)
		{
			if (animation == null || animation.State.IsPaused)
				return;
			if (animation.State.TimeToNextFrame < 0)
				return;
			if (_gameState.Cutscene.IsSkipping && animation.Configuration.Loops > 0)
			{
				animation.State.TimeToNextFrame = 0;
				while (animation.NextFrame()) ;
			}
			else
			{
				animation.State.TimeToNextFrame--;
				if (animation.State.TimeToNextFrame < 0)
					animation.NextFrame();
			}
		}
	}
}