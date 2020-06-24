using System.Collections.Concurrent;
using System.Diagnostics;
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
		private readonly IGame _game;
		private IObject _lastTarget;
		private Task _currentWalk;
		private int _counter = -1;
		private IFollowSettings _followSettings;
		private float? _newRoomX, _newRoomY;
        private IEntity _follower => Entity;
        private int _inUpdate; //For preventing re-entrancy

        public AGSFollowComponent(IGame game)
		{
			_game = game;
            game.Events.OnRepeatedlyExecute.Subscribe(onRepeatedlyExecute);
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
            TargetBeingFollowed = obj;
            if (currentTarget != null)
            {
                FollowTag.AddTag(currentTarget, _follower);
            }
			_followSettings = settings ?? new AGSFollowSettings ();
		}

        public async Task StopFollowingAsync()
        {
            Follow(null, _followSettings);
            await (_walk?.StopWalkingAsync() ?? Task.CompletedTask);
        }

        [Property(DisplayName = "Following")]
        public IObject TargetBeingFollowed { get; private set; }

		public override void Dispose ()
		{
			base.Dispose ();
            _game.Events.OnRepeatedlyExecute.Unsubscribe(onRepeatedlyExecute);
		}

        private async void onRepeatedlyExecute()
		{
            if (Interlocked.CompareExchange(ref _inUpdate, 1, 0) != 0) return;
            try
            {
                var walk = _walk;
                if (walk == null) return;
                var hasRoom = _hasRoom;
                var target = TargetBeingFollowed;
    			var currentWalk = _currentWalk;
    			var followSettings = _followSettings;
    			if (target == null || followSettings == null) 
    			{
                    _currentWalk = null;
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
    			_counter = MathUtils.Random().Next (_followSettings.MinWaitBetweenWalks, _followSettings.MaxWaitBetweenWalks);
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
    			setNextWalk(target, followSettings, walk);
            }
            finally
            {
                _inUpdate = 0;
            }
		}

        private void setNextWalk(IObject target, IFollowSettings settings, IWalkComponent walk)
		{
		    if (isInAllowedDistance(target, settings) && MathUtils.Random().Next(100) <= settings.StayPutPercentage)
		    {
		        return;
		    }

		    PointF targetPoint = PointF.Empty;
		    int retries = 3;
		    while (retries > 0)
		    {
		        targetPoint = getFollowTargetPoint(target, settings);
		        if (isWalkingDistanceValid(targetPoint, settings)) break;
		        retries--;
		    }

		    if (retries <= 0)
		    {
		        Debug.WriteLine($"FollowComponent: Failed to get a target point which is not too close");
		        return;
		    }

		    _currentWalk = walk.WalkAsync (new Position(targetPoint.X, targetPoint.Y));
		}

	    private PointF getFollowTargetPoint(IObject target, IFollowSettings settings)
	    {
	        if (MathUtils.Random().Next(100) <= settings.WanderOffPercentage)
	        {
	            return wanderOff(target, settings);
	        }

	        return follow(target, settings);
	    }

	    private PointF wanderOff(IObject target, IFollowSettings settings)
		{
            var room = _game.State.Room;
            bool isLimitlessRoom = room == null || MathUtils.FloatEquals(room.Limits.Width, float.MaxValue);
		    float minRoomX = isLimitlessRoom ? 0f : room.Limits.X;
		    float maxRoomX = isLimitlessRoom ? _game.Settings.VirtualResolution.Width : room.Limits.X + room.Limits.Width;
		    float minRoomY = isLimitlessRoom ? 0f : room.Limits.Y;
		    float maxRoomY = isLimitlessRoom ? _game.Settings.VirtualResolution.Height : room.Limits.Y + room.Limits.Height;
		    float x = getPointInRange(settings.MinXOffsetForWanderOff, settings.MaxXOffsetForWanderOff, _obj.X, target.X, minRoomX, maxRoomX, settings.StayOnTheSameSideForXPercentage);
		    float y = getPointInRange(settings.MinYOffsetForWanderOff, settings.MaxYOffsetForWanderOff, _obj.Y, target.Y, minRoomY, maxRoomY, settings.StayOnTheSameSideForYPercentage);

			return new PointF(x, y);
		}

		private PointF follow(IObject target, IFollowSettings settings)
		{
		    float x = getPointInRange(settings.MinXOffset, settings.MaxXOffset, _obj.X, target.X, -1f, -1f, settings.StayOnTheSameSideForXPercentage); 
		    float y = getPointInRange(settings.MinYOffset, settings.MaxYOffset, _obj.Y, target.Y, -1f, -1f, settings.StayOnTheSameSideForYPercentage); 
			return new PointF (x, y);
		}

	    private float getPointInRange(float? minOffset, float? maxOffset, float current, float target, float minRoom, float maxRoom, int stayOnSameSidePercentage)
	    {
	        float factor = 1f;
	        if (MathUtils.Random().Next(100) <= stayOnSameSidePercentage)
	        {
	            if (current < target) factor = -1f;
	        }
	        else if (current > target) factor = -1f;

	        float min = minOffset == null ? minRoom : target + (minOffset.Value * factor);
	        float max = maxOffset == null ? maxRoom : target + (maxOffset.Value * factor);
	        if (min > max)
	        {
	            float tmp = min;
	            min = max;
	            max = tmp;
	        }

	        return MathUtils.Random().Next((int)min, (int)max);
	    }

	    private bool isInAllowedDistance(IObject target, IFollowSettings settings)
	    {
	        return _obj.X.IsBetween(target.X + settings.MinXOffset, target.X + settings.MaxXOffset) &&
	               _obj.Y.IsBetween(target.Y + settings.MinYOffset, target.Y + settings.MaxYOffset);
	    }

	    private bool isWalkingDistanceValid(PointF targetPoint, IFollowSettings settings) => MathUtils.Distance((_obj.X, _obj.Y), targetPoint) >= settings.MinimumWalkingDistance;
	}
}
