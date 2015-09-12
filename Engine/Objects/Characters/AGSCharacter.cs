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
		private IObject obj;
		private CancellationTokenSource walkCancel;
		private IPathFinder pathFinder;
		private List<IImageRenderer> debugPath;

		public AGSCharacter (IObject obj = null, IPathFinder pathFinder = null)
		{
			this.pathFinder = pathFinder ?? new EPPathFinder ();
			this.obj = obj ?? new AGSObject(new AGSSprite());
			DebugDrawAnchor = true;
			walkCancel = new CancellationTokenSource ();
			debugPath = new List<IImageRenderer> ();
		}

		/// <summary>
		/// The larger this is, the slower all character walks will be.
		/// </summary>
		public static int MaxWalkDelay = 6;

		#region ICharacter implementation

		public float X { get { return obj.X; } set { obj.X = value; } }
		public float Y { get { return obj.Y; } set { obj.Y = value; } }
		public float Z { get { return obj.Z; } set { obj.Z = value; } }
		public IRenderLayer RenderLayer { get { return obj.RenderLayer; } set { obj.RenderLayer = value; } }

		public ITreeNode<IObject> TreeNode { get { return obj.TreeNode; } }

		public IImage Image { get { return obj.Image; } set { obj.Image = value; } }
		public IImageRenderer CustomRenderer 
		{ 
			get { return obj.CustomRenderer; } 
			set { obj.CustomRenderer = value; } 
		}

		public bool Enabled { get { return obj.Enabled; } set { obj.Enabled = value; } }
		public string Hotspot { get { return obj.Hotspot; } set { obj.Hotspot = value; } }

		public void ResetScale ()
		{
			obj.ResetScale ();
		}

		public void ScaleBy (float scaleX, float scaleY)
		{
			obj.ScaleBy (scaleX, scaleY);
		}

		public void ScaleTo (float width, float height)
		{
			obj.ScaleTo (width, height);
		}

		public ILocation Location { get { return obj.Location; } set { obj.Location = value; } }

		public float Height { get { return obj.Height; } }

		public float Width { get { return obj.Width; } }

		public float ScaleX { get { return obj.ScaleX; } }

		public float ScaleY { get { return obj.ScaleY; } }

		public float Angle {get { return obj.Angle;} set { obj.Angle = value;}}

		public byte Opacity {get { return obj.Opacity;} set { obj.Opacity = value;}}

		public Color Tint {get { return obj.Tint;} set { obj.Tint = value;}}

		public IPoint Anchor {get { return obj.Anchor;} set { obj.Anchor = value;}}

		public ISquare BoundingBox { get { return obj.BoundingBox; } set { obj.BoundingBox = value; } }

		public bool IgnoreViewport { get { return obj.IgnoreViewport; } set { obj.IgnoreViewport = value; } }

		public bool DebugDrawAnchor { get { return obj.DebugDrawAnchor; } set { obj.DebugDrawAnchor = value; } }
		public IBorderStyle Border { get { return obj.Border; } set { obj.Border = value; } }
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
			List<IImageRenderer> debugRenderers = debugPath;
			if (debugRenderers != null) 
			{
				foreach (IImageRenderer renderer in debugRenderers) 
				{
					//Room.RemoveCustomRenderer (renderer);
				}
			}
			walkCancel.Cancel ();
			CancellationTokenSource token = new CancellationTokenSource ();
			walkCancel = token;
			debugRenderers = DebugDrawWalkPath ? new List<IImageRenderer> () : null;
			debugPath = debugRenderers;

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
			obj.StartAnimation (animation);
		}

		public AnimationCompletedEventArgs Animate (IAnimation animation)
		{
			return obj.Animate (animation);
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

		public IRoom Room { get { return obj.Room; } set { obj.Room = value; } }

		public IAnimation Animation { get { return obj.Animation; } }

		public IInteractions Interactions { get { return obj.Interactions; } }

		public bool Visible { get { return obj.Visible; } set { obj.Visible = value; } }

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
			if (!isWalkable (destination)) 
			{
				IPoint closest = getClosestWalkablePoint (destination);
				if (closest == null)
					return new List<ILocation> ();
				destination = new AGSLocation (closest, destination.Z);
			}
			bool[][] mask = getWalkableMask ();
			pathFinder.Init (mask);
			return pathFinder.GetWalkPoints (Location, destination);
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
			int maxWidth = Room.WalkableAreas.Max(a => a.Mask.Length);
			bool[][] mask = new bool[maxWidth][];
			foreach (var area in Room.WalkableAreas) 
			{
				area.ApplyToMask (mask);
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
			if (animation == obj.Animation)
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

