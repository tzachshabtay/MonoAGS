using System;
using AGS.API;
using System.Threading;

namespace AGS.Engine
{
	public class AGSGameLoop : IGameLoop
	{
		private readonly IGameState _gameState;
		private readonly Size _virtualResolution;
        private int _inUpdate; //For preventing re-entrancy
        private readonly IDisplayList _displayList;
        private readonly IInput _input;

		public AGSGameLoop (IGameState gameState, Size virtualResolution, 
                            IDisplayList displayList, IInput input)
		{
            _displayList = displayList;
			_gameState = gameState;
			_virtualResolution = virtualResolution;
            _input = input;
		}

		#region IGameLoop implementation

		public void Update(bool resetCamera)
		{
            if (Interlocked.CompareExchange(ref _inUpdate, 1, 0) != 0) return;
            try
            {
                if (_gameState.Room == null) return;
                updateViewports(resetCamera);
                _displayList.Update(resetCamera);
                updateCursor();
            }
            finally
            {
                _inUpdate = 0;
            }
		}

		#endregion

        private void updateViewports(bool resetCamera)
        {
            updateViewport(_gameState.Viewport, resetCamera);
            try
            {
                foreach (var viewport in _gameState.SecondaryViewports)
                {
                    updateViewport(viewport, resetCamera);
                }
            }
            catch (InvalidOperationException) { } //can be triggered if a viewport was added/removed while enumerating- this should be resolved on next tick
		}

        private void updateViewport (IViewport viewport, bool resetCamera)
		{
            runAnimations(viewport.RoomProvider.Room);
            viewport.Camera?.Tick(viewport, viewport.RoomProvider.Room.Limits, _virtualResolution, resetCamera);
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

        private void updateCursor()
        {
            IObject cursor = _gameState.Cursor;
            if (cursor == null) return;
            var viewport = _gameState.Viewport;
            cursor.X = _input.MousePosition.XMainViewport;
            cursor.Y = _input.MousePosition.YMainViewport;
            cursor.GetModelMatrices();
            cursor.GetBoundingBoxes(viewport);
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
				while (animation.NextFrame()) {}
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
