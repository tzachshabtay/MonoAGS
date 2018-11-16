using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using AGS.API;

namespace AGS.Engine
{
	public class AGSFollowComponent : AGSComponent, IFollowComponent
	{
		private IWalkComponent _walk;
		private IHasRoomComponent _hasRoom;
		private ITranslate _obj;
		private IGame _game;
		private IObject _lastTarget;
		private Task _currentWalk;
		private int _counter = -1;
		private IFollowSettings _followSettings;
		private float? _newRoomX, _newRoomY;
        private IEntity _follower => Entity;
        private ConcurrentQueue<TaskCompletionSource<object>> _stopFollowTasks = new ConcurrentQueue<TaskCompletionSource<object>>();

        public AGSFollowComponent(IGame game)
		{
			_game = game;
            game.Events.OnRepeatedlyExecute.SubscribeToAsync(onRepeatedlyExecute);
		}

		public override void Init ()
		{
			base.Init ();
            Entity.Bind<IWalkComponent>(c => _walk = c, _ => _walk = null);
            Entity.Bind<IHasRoomComponent>(c => _hasRoom = c, _ => _hasRoom = null);
            Entity.Bind<ITranslateComponent>(c => _obj = c, _ => _obj = null);
		}

		public void Follow (IObject obj, IFollowSettings settings)
		{
            var currentTarget = obj;
            var previosTarget = TargetBeingFollowed;
            if (previosTarget != null)
            {
                FollowTag.RemoveTag(previosTarget, _follower);
            }
            else releaseStopFollowTasks(); //todo: this is not thread safe -> if StopFollowingAsync is called after Follow was called but before previousTarget was assigned
            TargetBeingFollowed = obj;
            if (currentTarget != null)
            {
                FollowTag.AddTag(currentTarget, _follower);
            }
			_followSettings = settings ?? new AGSFollowSettings ();
		}

        public async Task StopFollowingAsync()
        {
            TaskCompletionSource<object> tcs = new TaskCompletionSource<object>();
            _stopFollowTasks.Enqueue(tcs);
            Follow(null, _followSettings);
            await tcs.Task;
        }

        [Property(DisplayName = "Following")]
        public IObject TargetBeingFollowed { get; private set; }

		public override void Dispose ()
		{
			base.Dispose ();
            _game.Events.OnRepeatedlyExecute.UnsubscribeToAsync(onRepeatedlyExecute);
		}

        private async Task onRepeatedlyExecute()
		{
            var walk = _walk;
            if (walk == null) return;
            var hasRoom = _hasRoom;
            var target = TargetBeingFollowed;
			var currentWalk = _currentWalk;
			var followSettings = _followSettings;
			if (target == null || followSettings == null) 
			{
				if (currentWalk != null) await walk.StopWalkingAsync();
                _currentWalk = null;
                releaseStopFollowTasks();
                return;
            }
			if (target == _lastTarget) 
			{
				if (!currentWalk?.IsCompleted ?? false) return;
			}
			_lastTarget = target;
			if (_counter > 0) 
			{
				if (hasRoom?.Room != target.Room && _newRoomX == null) 
				{
					_newRoomX = target.X;
					_newRoomY = target.Y;
				}
				_counter--;
				return;
			}
			_counter = MathUtils.Random ().Next (_followSettings.MinWaitBetweenWalks, _followSettings.MaxWaitBetweenWalks);
			if (hasRoom?.Room != target.Room) 
			{
				if (_followSettings.FollowBetweenRooms) 
				{
					await hasRoom.ChangeRoomAsync(target.Room, _newRoomX, _newRoomY);
					walk.PlaceOnWalkableArea ();
					_newRoomX = null;
					_newRoomY = null;
				}
				return;
			}
			setNextWalk (target, followSettings, walk);
		}

        private void releaseStopFollowTasks()
        {
            while (_stopFollowTasks.TryDequeue(out TaskCompletionSource<object> tcs))
                tcs.TrySetResult(null);
        }

        private void setNextWalk (IObject target, IFollowSettings settings, IWalkComponent walk)
		{
			PointF targetPoint;
			if (MathUtils.Random ().Next (100) <= settings.WanderOffPercentage) 
			{
				targetPoint = wanderOff ();
			} 
			else targetPoint = follow (target, settings);

            _currentWalk = walk.WalkAsync (new Position(targetPoint.X, targetPoint.Y));
		}

		private PointF wanderOff()
		{
            var room = _game.State.Room;
            bool isLimitlessRoom = MathUtils.FloatEquals(room?.Limits.Width ?? float.MaxValue, float.MaxValue);
            var minX = isLimitlessRoom ? 0f : room.Limits.X;
            var maxX = isLimitlessRoom ? _game.Settings.VirtualResolution.Width : room.Limits.X + room.Limits.Width;
            var minY = isLimitlessRoom ? 0f : room.Limits.Y;
            var maxY = isLimitlessRoom ? _game.Settings.VirtualResolution.Height : room.Limits.Y + room.Limits.Height;

            float x = (float)MathUtils.Random().Next((int)minX, (int)maxX);
			float y = (float)MathUtils.Random().Next((int)minY, (int)maxY);
			return new PointF(x, y);
		}

		private PointF follow(IObject target, IFollowSettings settings)
		{
			float yOffset = MathUtils.Lerp (0f, settings.MinYOffset, 1f, settings.MaxYOffset, (float)MathUtils.Random ().NextDouble ());
			float xOffset = MathUtils.Lerp (0f, settings.MinXOffset, 1f, settings.MaxXOffset, (float)MathUtils.Random ().NextDouble ());

            var obj = _obj;
			float x = obj != null && obj.X > target.X ? target.X + xOffset : target.X - xOffset;
			float y = obj != null && obj.Y > target.Y ? target.Y + yOffset : target.Y - yOffset;
			return new PointF (x, y);
		}
	}
}