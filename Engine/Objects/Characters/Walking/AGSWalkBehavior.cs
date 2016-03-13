using System;
using AGS.API;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Threading;
using System.Linq;
using System.Diagnostics;

namespace AGS.Engine
{
	public class AGSWalkBehavior : IWalkBehavior
	{
		private CancellationTokenSource _walkCancel;
		private TaskCompletionSource<object> _walkCompleted;
		private IPathFinder _pathFinder;
		private List<IObject> _debugPath;
		private IObject _obj;
		private IAGSFaceDirectionBehavior _faceDirection;
		private IOutfitHolder _outfit;
		private IObjectFactory _objFactory;
		private ICutscene _cutscene;
		private IGameState _state;

		public AGSWalkBehavior(IObject obj, IPathFinder pathFinder, IAGSFaceDirectionBehavior faceDirection, 
			IOutfitHolder outfit, IObjectFactory objFactory, IGameState state)
		{
			_cutscene = state.Cutscene;
			_obj = obj;
			_pathFinder = pathFinder;
			_faceDirection = faceDirection;
			_outfit = outfit;
			_objFactory = objFactory;
			_state = state;

			_walkCancel = new CancellationTokenSource ();
			_debugPath = new List<IObject> ();
			_walkCompleted = new TaskCompletionSource<object> ();
			_walkCompleted.SetResult(null);
            AdjustWalkSpeedToScaleArea = true;
		}

		/// <summary>
		/// The larger this is, the slower all character walks will be.
		/// </summary>
		public static int MaxWalkDelay = 6;

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

		public int WalkSpeed { get; set; }

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
			_walkCancel.Cancel ();
			CancellationTokenSource token = new CancellationTokenSource ();
			_walkCancel = token;
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

			_faceDirection.CurrentDirectionalAnimation = _outfit.Outfit.WalkAnimation;
			await _faceDirection.FaceDirectionAsync(_obj.X, _obj.Y, destination.X, destination.Y);

			if (_cutscene.IsSkipping)
			{
				_obj.X = destination.X;
				_obj.Y = destination.Y;
				return true;
			}
			float xSteps = Math.Abs (destination.X - _obj.X);
			float ySteps = Math.Abs (destination.Y - _obj.Y);

			float numSteps = Math.Max (xSteps, ySteps);

			float xStep = xSteps / numSteps;
			if (_obj.X > destination.X) xStep = -xStep;

			float yStep = ySteps / numSteps;
			if (_obj.Y > destination.Y) yStep = -yStep;

			int numStepsInt = (int)numSteps;
			int delay = MaxWalkDelay - WalkSpeed;
            delay = adjustWalkSpeed(delay);

			for (int step = 0; step < numStepsInt; step++) 
			{
				if (token.IsCancellationRequested)
					return false;

				_obj.X += xStep;
				_obj.Y += yStep;
				await Task.Delay(delay);
			}
			_obj.X = destination.X;
			_obj.Y = destination.Y;
			return true;
		}
        
		private int adjustWalkSpeed(int delay)
		{
			delay = adjustWalkSpeedBasedOnArea(delay);
			if (_state.Speed != 100 && _state.Speed > 0)
			{
				float factor = (float)_state.Speed / 100f;
				delay = (int)(delay / factor);
			}
			return delay;
		}

        private int adjustWalkSpeedBasedOnArea(int delay)
        {
			if (_obj == null || _obj.Room == null || _obj.Room.ScalingAreas == null ||
				_obj.IgnoreScalingArea || !AdjustWalkSpeedToScaleArea) return delay;
            
            foreach (var area in _obj.Room.ScalingAreas)
            {
                if (!area.Enabled || !area.ScaleObjects || !area.IsInArea(_obj.Location)) continue;
                float scale = area.GetScaling(_obj.Y);
                if (scale != 1f)
                {
                    delay = (int)(((float)delay) / scale);
                    if (delay == 0) delay = 1;
                }
                break;
            }
            return delay;
        }
	}
}

