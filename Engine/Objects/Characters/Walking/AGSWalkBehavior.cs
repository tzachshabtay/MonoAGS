using System;
using AGS.API;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Threading;
using System.Linq;

namespace AGS.Engine
{
	public class AGSWalkBehavior : AGSComponent, IWalkBehavior
	{
        private TaskCompletionSource<object> _walkCompleted;
        private WalkLineInstruction _currentWalkLine;
        private IPathFinder _pathFinder;
		private List<IObject> _debugPath;
		private IObject _obj;
		private IFaceDirectionBehavior _faceDirection;
		private IHasOutfit _outfit;
		private IObjectFactory _objFactory;
		private ICutscene _cutscene;
		private IGameState _state;
        private IGameEvents _events;
        
		public AGSWalkBehavior(IObject obj, IPathFinder pathFinder, IFaceDirectionBehavior faceDirection, 
			IHasOutfit outfit, IObjectFactory objFactory, IGame game)
		{
            _state = game.State;
            _cutscene = _state.Cutscene;
            _events = game.Events;
			_obj = obj;
			_pathFinder = pathFinder;
			_faceDirection = faceDirection;
			_outfit = outfit;
			_objFactory = objFactory;

			_debugPath = new List<IObject> ();
			_walkCompleted = new TaskCompletionSource<object> ();
			_walkCompleted.SetResult(null);
            AdjustWalkSpeedToScaleArea = true;

            _events.OnRepeatedlyExecute.Subscribe(onRepeatedlyExecute);
		}

        public static int WalkLineTimeoutInMilliseconds = 15000; //15 seconds

        public override void Dispose()
        {
            base.Dispose();
            _events.OnRepeatedlyExecute.Unsubscribe(onRepeatedlyExecute);
        }

        #region IWalkBehavior implementation

        public bool Walk (ILocation location)
		{
			return Task.Run (async () => await WalkAsync (location)).Result;
		}

		public async Task<bool> WalkAsync (ILocation location)	
		{
			List<IObject> debugRenderers = _debugPath;
			if (debugRenderers != null) 
			{
				foreach (var renderer in debugRenderers) 
				{
					renderer.ChangeRoom(null);
				}
			}
			CancellationTokenSource token = await stopWalkingAsync();
			debugRenderers = DebugDrawWalkPath ? new List<IObject> () : null;
			_debugPath = debugRenderers;
			_walkCompleted = new TaskCompletionSource<object> (null);
			float xSource = _obj.X;
			float ySource = _obj.Y;
			bool completedWalk = false;
			Exception caught = null;
			try
			{
				completedWalk = await walkAsync(location, token, debugRenderers);
			}
			catch (Exception e)
			{
				caught = e;
			}

			//On windows (assuming we're before c# 6.0), we can't await inside a finally, so we're using the workaround pattern
			_faceDirection.CurrentDirectionalAnimation = _outfit.Outfit.IdleAnimation;
			await _faceDirection.FaceDirectionAsync(_faceDirection.Direction);
			_walkCompleted.TrySetResult(null);

			if (caught != null) throw caught;

			return completedWalk;
		}

		public void StopWalking()
		{
			Task.Run(async () => await StopWalkingAsync()).Wait();
		}

		public async Task StopWalkingAsync()
		{
			await stopWalkingAsync();
		}

		public void PlaceOnWalkableArea()
		{
			AGSPoint current = new AGSPoint (_obj.X, _obj.Y);
			IPoint closestPoint = getClosestWalkablePoint (current);
			if (closestPoint != null) 
			{
				_obj.X = closestPoint.X;
				_obj.Y = closestPoint.Y;
			}
		}

		public float WalkSpeed { get; set; }

        public bool AdjustWalkSpeedToScaleArea { get; set; }
        
		public bool IsWalking
		{ 
			get
			{ 
				Task task = _walkCompleted.Task;
				return (!task.IsCompleted && !task.IsCanceled && !task.IsFaulted);
			}
		}

		public bool DebugDrawWalkPath { get; set; }

        #endregion

        private void onRepeatedlyExecute(object sender, AGSEventArgs args)
        {
            WalkLineInstruction currentLine = _currentWalkLine;
            if (currentLine == null) return;

            if (currentLine.CancelToken.IsCancellationRequested || currentLine.NumSteps <= 0)
            {
                _currentWalkLine = null; //Possible race condition here? If so, need to replace with concurrent queue
                currentLine.OnCompletion.TrySetResult(null);                
                return;
            }

            currentLine.NumSteps--;
            _obj.X += currentLine.XStep;
            _obj.Y += currentLine.YStep;
        }

		private async Task<bool> walkAsync(ILocation location, CancellationTokenSource token, List<IObject> debugRenderers)
		{
			IEnumerable<ILocation> walkPoints = getWalkPoints (location);

			if (!walkPoints.Any ()) return false;
			foreach (var point in walkPoints) 
			{
				if (point.X == _obj.X && point.Y == _obj.Y) continue;
				if (!await walkStraightLine (point, token, debugRenderers)) return false;
			}
			return true;
		}

		private async Task<CancellationTokenSource> stopWalkingAsync()
		{
            var currentLine = _currentWalkLine;
            if (currentLine != null)
            {
                currentLine.CancelToken.Cancel();
                await currentLine.OnCompletion.Task;
            }
			CancellationTokenSource token = new CancellationTokenSource ();
			await _walkCompleted.Task;
			return token;
		}

		private IPoint getClosestWalkablePoint(IPoint target)

		{
			IPoint closestPoint = null;
			float closestDistance = float.MaxValue;
			foreach (IArea area in _obj.Room.WalkableAreas) 
			{
				if (!area.Enabled) continue;
				float distance;
				IPoint point = area.FindClosestPoint (target, out distance);
				if (distance < closestDistance) 
				{
					closestPoint = point;
					closestDistance = distance;
				}
			}
			return closestPoint;
		}

		private IEnumerable<ILocation> getWalkPoints(ILocation destination)
		{
			if (!isWalkable(_obj.Location))
				return new List<ILocation> ();
			if (!isWalkable (destination)) 
			{
				IPoint closest = getClosestWalkablePoint (destination);
				if (closest == null)
					return new List<ILocation> ();
				destination = new AGSLocation (closest, destination.Z);
			}
			bool[][] mask = getWalkableMask ();
			_pathFinder.Init (mask);
			return _pathFinder.GetWalkPoints (_obj.Location, destination);
		}

		private bool isWalkable(ILocation location)
		{
			foreach (var area in _obj.Room.WalkableAreas) 
			{
				if (area.IsInArea (location))
					return true;
			}
			return false;
		}

		private bool[][] getWalkableMask()
		{
			int maxWidth = _obj.Room.WalkableAreas.Max(a => a.Mask.Width);
			bool[][] mask = new bool[maxWidth][];
			foreach (var area in _obj.Room.WalkableAreas) 
			{
				area.Mask.ApplyToMask (mask);
			}
			return mask;
		}

		private async Task<bool> walkStraightLine(ILocation destination, 
			CancellationTokenSource token, List<IObject> debugRenderers)
		{
			if (debugRenderers != null) 
			{
				GLLineRenderer line = new GLLineRenderer (_obj.X, _obj.Y, destination.X, destination.Y);
				IObject renderer = _objFactory.GetObject("Debug Line");
				renderer.CustomRenderer = line;
				renderer.ChangeRoom(_obj.Room);
				debugRenderers.Add (renderer);
			}

			if (!isDistanceVeryShort(destination))
			{
				var lastDirection = _faceDirection.Direction;
				_faceDirection.CurrentDirectionalAnimation = _outfit.Outfit.WalkAnimation;
				await _faceDirection.FaceDirectionAsync(_obj.X, _obj.Y, destination.X, destination.Y);
				if (lastDirection != _faceDirection.Direction)
				{
					await Task.Delay(200);
				}
			}

			if (_cutscene.IsSkipping)
			{
				_obj.X = destination.X;
				_obj.Y = destination.Y;
				return true;
			}
			float xSteps = Math.Abs (destination.X - _obj.X);
			float ySteps = Math.Abs (destination.Y - _obj.Y);

			float numSteps = Math.Max (xSteps, ySteps) / adjustWalkSpeed(WalkSpeed);

            float xStep = xSteps / numSteps;
			if (_obj.X > destination.X) xStep = -xStep;

			float yStep = ySteps / numSteps;
			if (_obj.Y > destination.Y) yStep = -yStep;

			int numStepsInt = (int)numSteps;

            WalkLineInstruction instruction = new WalkLineInstruction(token, numStepsInt, xStep, yStep);
            _currentWalkLine = instruction;
            Task timeout = Task.Delay(WalkLineTimeoutInMilliseconds);
            Task completedTask = await Task.WhenAny(_currentWalkLine.OnCompletion.Task, timeout);

            if (completedTask == timeout)
            {
                instruction.CancelToken.Cancel();
                return false;
            }

            if (instruction.CancelToken.IsCancellationRequested)
                return false;
			
			_obj.X = destination.X;
			_obj.Y = destination.Y;
			return true;
		}

		private bool isDistanceVeryShort(ILocation destination)
		{
			var deltaX = destination.X - _obj.X;
			var deltaY = destination.Y - _obj.Y;
			return (deltaX * deltaX) + (deltaY * deltaY) < 10;
		}

        private float adjustWalkSpeed(float walkSpeed)
		{
            walkSpeed = adjustWalkSpeedBasedOnArea(walkSpeed);
			return walkSpeed;
		}

        private float adjustWalkSpeedBasedOnArea(float walkSpeed)
        {
			if (_obj == null || _obj.Room == null || _obj.Room.ScalingAreas == null ||
				_obj.IgnoreScalingArea || !AdjustWalkSpeedToScaleArea) return walkSpeed;
            
            foreach (var area in _obj.Room.ScalingAreas)
            {
                if (!area.Enabled || !area.ScaleObjects || !area.IsInArea(_obj.Location)) continue;
                float scale = area.GetScaling(_obj.Y);
                if (scale != 1f)
                {
                    walkSpeed *= scale;
                    if (walkSpeed == 0) walkSpeed = 1;
                }
                break;
            }
            return walkSpeed;
        }

        private class WalkLineInstruction
        {
            public WalkLineInstruction(CancellationTokenSource token, int numSteps, float xStep, float yStep)
            {
                CancelToken = token;
                NumSteps = numSteps;
                XStep = xStep;
                YStep = yStep;
                OnCompletion = new TaskCompletionSource<object>();
            }

            public CancellationTokenSource CancelToken { get; private set; }
            public TaskCompletionSource<object> OnCompletion { get; private set; }
            public int NumSteps { get; set; }
            public float XStep { get; private set; }
            public float YStep { get; private set; }
        }
    }
}

