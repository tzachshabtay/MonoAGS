using System;
using API;
using System.Threading.Tasks;

namespace Engine
{
	public class AGSGameLoop : IGameLoop
	{
		IGameState gameState;

		public AGSGameLoop (IGameState gameState)
		{
			this.gameState = gameState;
		}

		#region IGameLoop implementation

		public virtual void Update ()
		{
			if (gameState.Player.Character == null) return;
			IRoom room = gameState.Player.Character.Room;
			if (room.Background != null) runAnimation (room.Background.Animation);
			foreach (var obj in room.Objects) 
			{
				if (!obj.Visible)
					continue;
				if (!room.ShowPlayer && obj == gameState.Player.Character)
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
				IPoint point = viewportFollower.Follow (new AGSPoint (room.Viewport.X, room.Viewport.Y));
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

