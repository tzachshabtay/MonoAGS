using System;
using AGS.API;
using System.Threading.Tasks;
using System.Drawing;

namespace AGS.Engine
{
	public class AGSObject : IObject
	{
		private IAnimationContainer _animation;

		/// <summary>
		/// Initializes a new instance of the <see cref="Engine.AGSObject"/> class.
		/// Width and height will be set based on the first animation frame (or single image) used.
		/// </summary>
		public AGSObject (IAnimationContainer animationContainer, IInteractions interactions)
		{
			_animation = animationContainer;
			Interactions = interactions;

			Enabled = true;
			RenderLayer = AGSLayers.Foreground;
			TreeNode = new AGSTreeNode<IObject> (this);
			IgnoreScalingArea = true;
		}
			
		#region IObject implementation

		public ITreeNode<IObject> TreeNode { get; private set; }

		public void ResetScale ()
		{
			_animation.ResetScale();
		}

		public void ScaleBy (float scaleX, float scaleY)
		{
			_animation.ScaleBy(scaleX, scaleY);
		}

		public void ScaleTo (float width, float height)
		{
			_animation.ScaleTo(width, height);
		}

		public void FlipHorizontally()
		{
			_animation.FlipHorizontally();
		}

		public void FlipVertically()
		{
			_animation.FlipVertically();
		}

		public ISprite Clone()
		{
			return _animation.Clone();
		}

		public ILocation Location { get { return _animation.Location; } set { _animation.Location = value; } }

		public float Height { get { return _animation.Height; } }

		public float Width { get { return _animation.Width; } }

		public float ScaleX { get { return _animation.ScaleX; } }

		public float ScaleY { get { return _animation.ScaleY; } }

		public float Angle { get { return _animation.Angle; } set { _animation.Angle = value; } }

		public byte Opacity { get { return _animation.Opacity; } set { _animation.Opacity = value; } }

		public Color Tint { get { return _animation.Tint; } set { _animation.Tint = value; } }

		public IPoint Anchor { get { return _animation.Anchor; } set { _animation.Anchor = value; } }

		public ISquare BoundingBox { get; set; }
		public IArea PixelPerfectHitTestArea  { get { return _animation.PixelPerfectHitTestArea; } }
		public void PixelPerfect(bool pixelPerfect)
		{
			_animation.PixelPerfect(pixelPerfect);
		}

		public float X { get { return _animation.X; } set { _animation.X = value; } }
		public float Y { get { return _animation.Y; } set { _animation.Y = value; } }
		public float Z { get { return _animation.Z; } set { _animation.Z = value; } }

		public IRenderLayer RenderLayer { get; set; }

		public bool IgnoreViewport { get; set; }
		public bool IgnoreScalingArea { get; set; }

		public IImage Image { get { return _animation.Image; } set { _animation.Image = value; } }

		public IImageRenderer CustomRenderer 
		{ 
			get { return _animation.CustomRenderer; } 
			set { _animation.CustomRenderer = value; } 
		}

		public void StartAnimation(IAnimation animation)
		{
			_animation.StartAnimation(animation);
		}

		public AnimationCompletedEventArgs Animate (IAnimation animation)
		{
			return _animation.Animate(animation);
		}

		public async Task<AnimationCompletedEventArgs> AnimateAsync (IAnimation animation)
		{
			return await _animation.AnimateAsync(animation);
		}

		public void ChangeRoom(IRoom newRoom, float? x = null, float? y = null)
		{
			if (Room != null)
			{
				Room.Objects.Remove(this);
			}
			newRoom.Objects.Add(this);
			Room = newRoom;

			if (x != null) X = x.Value;
			if (y != null) Y = y.Value;
		}

		public IRoom Room { get; set; }

		public IAnimation Animation { get { return _animation.Animation; } }

		public IInteractions Interactions { get; private set; }

		public bool Visible { get { return _animation.Visible; } set { _animation.Visible = value; } }

		public bool Enabled { get; set; }

		public string Hotspot { get; set; }

		public bool DebugDrawAnchor { get { return _animation.DebugDrawAnchor; } set { _animation.DebugDrawAnchor = value; } }

		public IBorderStyle Border { get { return _animation.Border; } set { _animation.Border = value; } }

		public override string ToString ()
		{
			return Hotspot ?? base.ToString ();
		}

		#endregion
	}
}

