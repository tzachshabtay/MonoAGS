using System;
using AGS.API;
using System.Threading.Tasks;
using System.Drawing;

namespace AGS.Engine
{
	public class AGSGameLoop : IGameLoop
	{
		IGameState _gameState;
		Size _virtualResolution;

		public AGSGameLoop (IGameState gameState, Size virtualResolution)
		{
			this._gameState = gameState;
			this._virtualResolution = virtualResolution;
		}

		#region IGameLoop implementation

		public virtual void Update ()
		{
			if (_gameState.Player.Character == null) return;
			IRoom room = _gameState.Player.Character.Room;
			if (room.Background != null) runAnimation (room.Background.Animation);
			foreach (var obj in room.Objects) 
			{
				if (!obj.Visible)
					continue;
				if (!room.ShowPlayer && obj == _gameState.Player.Character)
					continue;
				runAnimation (obj.Animation);
			}

			updateViewport (room);
			Task.Run (async () => await UpdateAsync ()).Wait ();
		}

		#endregion

		protected virtual Task UpdateAsync ()
		{
			return Task.FromResult (true);
		}
			
		private void updateViewport (IRoom room)
		{
			IFollower viewportFollower = room.Viewport.Follower;
			if (viewportFollower != null) 
			{
				ISprite sprite = room.Background.Animation.Sprite;
				IPoint point = viewportFollower.Follow (new AGSPoint (room.Viewport.X, room.Viewport.Y),
					new Size((int)sprite.Width, (int)sprite.Height), _virtualResolution);
				room.Viewport.X = point.X;
				room.Viewport.Y = point.Y;
			}
		}

		private void runAnimation(IAnimation animation)
		{
			if (animation == null)
				return;
			if (animation.State.TimeToNextFrame < 0)
				return;
			animation.State.TimeToNextFrame--;
			if (animation.State.TimeToNextFrame < 0)
				animation.NextFrame ();
		}
	}
}

