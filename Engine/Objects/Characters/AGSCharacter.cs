using System;
using API;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Threading;
using System.Linq;
using System.Drawing;

namespace Engine
{
	public class AGSCharacter : ICharacter
	{
		private IObject _obj;
		private CancellationTokenSource _walkCancel;
		private IPathFinder _pathFinder;
		private List<IImageRenderer> _debugPath;

		public AGSCharacter (IObject obj = null, IPathFinder pathFinder = null)
		{
			this._pathFinder = pathFinder ?? new EPPathFinder ();
			this._obj = obj ?? new AGSObject(new AGSSprite());
			DebugDrawAnchor = true;
			_walkCancel = new CancellationTokenSource ();
			_debugPath = new List<IImageRenderer> ();
		}

		/// <summary>
		/// The larger this is, the slower all character walks will be.
		/// </summary>
		public static int MaxWalkDelay = 6;

		#region ICharacter implementation

		public float X { get { return _obj.X; } set { _obj.X = value; } }
		public float Y { get { return _obj.Y; } set { _obj.Y = value; } }
		public float Z { get { return _obj.Z; } set { _obj.Z = value; } }
		public IRenderLayer RenderLayer { get { return _obj.RenderLayer; } set { _obj.RenderLayer = value; } }

		public ITreeNode<IObject> TreeNode { get { return _obj.TreeNode; } }

		public IImage Image { get { return _obj.Image; } set { _obj.Image = value; } }
		public IImageRenderer CustomRenderer 
		{ 
			get { return _obj.CustomRenderer; } 
			set { _obj.CustomRenderer = value; } 
		}

		public bool Enabled { get { return _obj.Enabled; } set { _obj.Enabled = value; } }
		public string Hotspot { get { return _obj.Hotspot; } set { _obj.Hotspot = value; } }

		public void ResetScale ()
		{
			_obj.ResetScale ();
		}

		public void ScaleBy (float scaleX, float scaleY)
		{
			_obj.ScaleBy (scaleX, scaleY);
		}

		public void ScaleTo (float width, float height)
		{
			_obj.ScaleTo (width, height);
		}

		public void FlipHorizontally()
		{
			_obj.FlipHorizontally();
		}

		public void FlipVertically()
		{
			_obj.FlipVertically();
		}

		public ISprite Clone()
		{
			return _obj.Clone();
		}

		public ILocation Location { get { return _obj.Location; } set { _obj.Location = value; } }

		public float Height { get { return _obj.Height; } }

		public float Width { get { return _obj.Width; } }

		public float ScaleX { get { return _obj.ScaleX; } }

		public float ScaleY { get { return _obj.ScaleY; } }

		public float Angle {get { return _obj.Angle;} set { _obj.Angle = value;}}

		public byte Opacity {get { return _obj.Opacity;} set { _obj.Opacity = value;}}

		public Color Tint {get { return _obj.Tint;} set { _obj.Tint = value;}}

		public IPoint Anchor {get { return _obj.Anchor;} set { _obj.Anchor = value;}}

		public ISquare BoundingBox { get { return _obj.BoundingBox; } set { _obj.BoundingBox = value; } }

		public bool IgnoreViewport { get { return _obj.IgnoreViewport; } set { _obj.IgnoreViewport = value; } }

		public bool DebugDrawAnchor { get { return _obj.DebugDrawAnchor; } set { _obj.DebugDrawAnchor = value; } }
		public IBorderStyle Border { get { return _obj.Border; } set { _obj.Border = value; } }
		public bool DebugDrawWalkPath { get; set; }

		public void Say (string text)
		{
			throw new NotImplementedException ();
		}

		public Task SayAsync (string text)
		{
			throw new NotImplementedException ();
		}

		public bool Walk (ILocation location)
		{
			return Task.Run (async () => await WalkAsync (location)).Result;
		}

		public async Task<bool> WalkAsync (ILocation location)	
		{
			List<IImageRenderer> debugRenderers = _debugPath;
			if (debugRenderers != null) 
			{
				foreach (IImageRenderer renderer in debugRenderers) 
				{
					//Room.RemoveCustomRenderer (renderer);
				}
			}
			_walkCancel.Cancel ();
			CancellationTokenSource token = new CancellationTokenSource ();
			_walkCancel = token;
			debugRenderers = DebugDrawWalkPath ? new List<IImageRenderer> () : null;
			_debugPath = debugRenderers;

			float xSource = X;
			float ySource = Y;
			IEnumerable<ILocation> walkPoints = getWalkPoints (location);

			if (!walkPoints.Any ()) return false;
			foreach (var point in walkPoints) 
			{
				if (!await walkStraightLine (point, token, debugRenderers)) return false;
			}
			await setDirection (xSource, ySource, location.X, location.Y, IdleAnimation);
			return true;
		}

		public IInventory Inventory { get; set; }

		public IDirectionalAnimation WalkAnimation { get; set; }

		public IDirectionalAnimation IdleAnimation { get; set; }

		public ITextConfig SpeechTextConfig { get; set; }

		public int WalkSpeed { get; set; }

		#endregion

		#region IObject implementation

		public void StartAnimation (IAnimation animation)
		{
			_obj.StartAnimation (animation);
		}

		public AnimationCompletedEventArgs Animate (IAnimation animation)
		{
			return _obj.Animate (animation);
		}

		public async Task<AnimationCompletedEventArgs> AnimateAsync (IAnimation animation)
		{
			return await AnimateAsync (animation);
		}

		public void PlaceOnWalkableArea()
		{
			AGSPoint current = new AGSPoint (X, Y);
			IPoint closestPoint = getClosestWalkablePoint (current);
			if (closestPoint != null) 
			{
				X = closestPoint.X;
				Y = closestPoint.Y;
			}
		}

		public IRoom Room { get { return _obj.Room; } set { _obj.Room = value; } }

		public IAnimation Animation { get { return _obj.Animation; } }

		public IInteractions Interactions { get { return _obj.Interactions; } }

		public bool Visible { get { return _obj.Visible; } set { _obj.Visible = value; } }

		public override string ToString ()
		{
			return Hotspot ?? base.ToString ();
		}

		#endregion

		private IPoint getClosestWalkablePoint(IPoint target)
		{
			IPoint closestPoint = null;
			float closestDistance = float.MaxValue;
			foreach (IArea area in Room.WalkableAreas) 
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
			if (!isWalkable(Location))
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
			return _pathFinder.GetWalkPoints (Location, destination);
		}

		private bool isWalkable(ILocation location)
		{
			foreach (var area in Room.WalkableAreas) 
			{
				if (area.IsInArea (location))
					return true;
			}
			return false;
		}

		private bool[][] getWalkableMask()
		{
			int maxWidth = Room.WalkableAreas.Max(a => a.Mask.Width);
			bool[][] mask = new bool[maxWidth][];
			foreach (var area in Room.WalkableAreas) 
			{
				area.Mask.ApplyToMask (mask);
			}
			return mask;
		}

		private async Task<bool> walkStraightLine(ILocation destination, 
			CancellationTokenSource token, List<IImageRenderer> debugRenderers)
		{
			if (debugRenderers != null) 
			{
				GLLineRenderer line = new GLLineRenderer (X, Y, destination.X, destination.Y);
				//Room.AddCustomRenderer (line);
				debugRenderers.Add (line);
			}

			await setDirection (X, Y, destination.X, destination.Y, WalkAnimation);
			float xSteps = Math.Abs (destination.X - X);
			float ySteps = Math.Abs (destination.Y - Y);

			float numSteps = Math.Max (xSteps, ySteps);

			float xStep = xSteps / numSteps;
			if (X > destination.X) xStep = -xStep;

			float yStep = ySteps / numSteps;
			if (Y > destination.Y) yStep = -yStep;

			int numStepsInt = (int)numSteps;
			int delay = MaxWalkDelay - WalkSpeed;

			for (int step = 0; step < numStepsInt; step++) 
			{
				if (token.IsCancellationRequested)
					return false;
				X += xStep;
				Y += yStep;
				await Task.Delay(delay);
			}
			X = destination.X;
			Y = destination.Y;
			return true;
		}

		private async Task setDirection(float xSource, float ySource, float xDest, float yDest, 
			IDirectionalAnimation animation)
		{
			float angle = getAngle (xSource, ySource, xDest, yDest);

			if (angle < -30 && angle > -60 && animation.DownRight != null)
				await changeAnimationIfNeeded (animation.DownRight);
			else if (angle < -120 && angle > -150 && animation.DownLeft != null)
				await changeAnimationIfNeeded (animation.DownLeft);
			else if (angle > 120 && angle < 150 && animation.UpLeft != null)
				await changeAnimationIfNeeded (animation.UpLeft);
			else if (angle > 30 && angle < 60 && animation.UpRight != null)
				await changeAnimationIfNeeded (animation.UpRight);
			else if (angle < -75 && angle > -105 && animation.Down != null)
				await changeAnimationIfNeeded (animation.Down);
			else if (angle > 75 && angle < 105 && animation.Up != null)
				await changeAnimationIfNeeded (animation.Up);
			else if (xDest > xSource && animation.Right != null) 
				await changeAnimationIfNeeded (animation.Right);
			else if (animation.Left != null)
				await changeAnimationIfNeeded(animation.Left);
		}

		private async Task changeAnimationIfNeeded(IAnimation animation)
		{
			if (animation == _obj.Animation)
				return;
			await Task.Delay (1);
			StartAnimation (animation);
			await Task.Delay (1);
		}

		private float getAngle(float x1, float y1, float x2, float y2)
		{
			float deltaX = x2 - x1;
			if (deltaX == 0f)
				deltaX = 0.001f;
			float deltaY = y2 - y1;
			float angle = ((float)Math.Atan2 (deltaY, deltaX)) * 180f / (float)Math.PI;
			return angle;
		}
	}
}

