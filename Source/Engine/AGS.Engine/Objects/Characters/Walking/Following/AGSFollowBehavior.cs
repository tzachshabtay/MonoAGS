using System.Threading.Tasks;
using AGS.API;

namespace AGS.Engine
{
	public class AGSFollowBehavior : AGSComponent, IFollowBehavior
	{
		private IWalkBehavior _walk;
		private IHasRoom _hasRoom;
		private ITranslate _obj;
		private IGame _game;
		private IObject _lastTarget;
		private Task _currentWalk;
		private int _counter = -1;
		private IFollowSettings _followSettings;
		private float? _newRoomX, _newRoomY;
        private IEntity _follower;

		public AGSFollowBehavior(IGame game)
		{
			_game = game;
            game.Events.OnRepeatedlyExecute.SubscribeToAsync(onRepeatedlyExecute);
		}

		public override void Init (IEntity entity)
		{
			base.Init (entity);
            _follower = entity;
            entity.Bind<IWalkBehavior>(c => _walk = c, _ => _walk = null);
            entity.Bind<IHasRoom>(c => _hasRoom = c, _ => _hasRoom = null);
            entity.Bind<ITranslateComponent>(c => _obj = c, _ => _obj = null);
		}

		public void Follow (IObject obj, IFollowSettings settings)
		{
            var currentTarget = obj;
            if (currentTarget != null)
            {
                FollowTag.RemoveTag(currentTarget, _follower);
            }
            TargetBeingFollowed = obj;
            FollowTag.AddTag(obj, _follower);
			_followSettings = settings ?? new AGSFollowSettings ();
		}

        public IObject TargetBeingFollowed { get; private set; }

		public override void Dispose ()
		{
			base.Dispose ();
            _game.Events.OnRepeatedlyExecute.UnsubscribeToAsync(onRepeatedlyExecute);
		}

        private async Task onRepeatedlyExecute ()
		{
            var walk = _walk;
            if (walk == null) return;
            var hasRoom = _hasRoom;
            var target = TargetBeingFollowed;
			var currentWalk = _currentWalk;
			var followSettings = _followSettings;
			if (target == null || followSettings == null) 
			{
				if (currentWalk != null) walk.StopWalking ();
				return;
			}
			if (target == _lastTarget) 
			{
				if (currentWalk != null && !currentWalk.IsCompleted) return;
			}
			_lastTarget = target;
			if (_counter > 0) 
			{
				if (hasRoom != null && hasRoom.Room != target.Room && _newRoomX == null) 
				{
					_newRoomX = target.X;
					_newRoomY = target.Y;
				}
				_counter--;
				return;
			}
			_counter = MathUtils.Random ().Next (_followSettings.MinWaitBetweenWalks, _followSettings.MaxWaitBetweenWalks);
			if (hasRoom != null && hasRoom.Room != target.Room) 
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

        private void setNextWalk (IObject target, IFollowSettings settings, IWalkBehavior walk)
		{
			PointF targetPoint;
			if (MathUtils.Random ().Next (100) <= settings.WanderOffPercentage) 
			{
				targetPoint = wanderOff ();
			} 
			else targetPoint = follow (target, settings);

			_currentWalk = walk.WalkAsync (new AGSLocation (targetPoint.X, targetPoint.Y));
		}

		private PointF wanderOff()
		{
			float x = (float)MathUtils.Random().Next(0, _game.Settings.VirtualResolution.Width);
			float y = (float)MathUtils.Random ().Next (0, _game.Settings.VirtualResolution.Height);
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

