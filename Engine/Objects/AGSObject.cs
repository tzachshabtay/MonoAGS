using System;
using AGS.API;
using System.Threading.Tasks;
using System.Drawing;
using Autofac;

namespace AGS.Engine
{
	public class AGSObject : IObject
	{
		private IAnimationContainer _animation;
		private readonly IGameState _state;
		private Lazy<IRoom> _cachedRoom;
		private bool _visible;
		private bool _enabled;

		/// <summary>
		/// Initializes a new instance of the <see cref="Engine.AGSObject"/> class.
		/// Width and height will be set based on the first animation frame (or single image) used.
		/// </summary>
		public AGSObject (IAnimationContainer animationContainer, IGameEvents gameEvents, Resolver resolver)
		{
			_animation = animationContainer;

			if (resolver != null)
			{
				TypedParameter defaults = new TypedParameter (typeof(IInteractions), gameEvents.DefaultInteractions);
				TypedParameter objParam = new TypedParameter (typeof(IObject), this);
				Interactions = resolver.Container.Resolve<IInteractions>(defaults, objParam);

				_state = resolver.Container.Resolve<IGameState>();
			}
			refreshRoom();

			Enabled = true;
			Visible = true;
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

		public IPoint WalkPoint { get; set; }

		public IPoint CenterPoint
		{
			get
			{
				float minX = BoundingBox.MinX;
				float minY = BoundingBox.MinY;
				float offsetX = PixelPerfectHitTestArea == null ? (BoundingBox.MaxX - BoundingBox.MinX) / 2f : 
					PixelPerfectHitTestArea.Mask.MinX + (PixelPerfectHitTestArea.Mask.MaxX - PixelPerfectHitTestArea.Mask.MinX) / 2f;
				float offsetY = PixelPerfectHitTestArea == null ? (BoundingBox.MaxY - BoundingBox.MinY) / 2f : 
					PixelPerfectHitTestArea.Mask.MinY + (PixelPerfectHitTestArea.Mask.MaxY - PixelPerfectHitTestArea.Mask.MinY) / 2f;

				return new AGSPoint (minX + offsetX, minY + offsetY);
			}
		}

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
			refreshRoom();

			if (x != null) X = x.Value;
			if (y != null) Y = y.Value;
		}

		public IRoom Room  { get { return _cachedRoom.Value; } }

		public IAnimation Animation { get { return _animation.Animation; } }

		public IInteractions Interactions { get; private set; }

		public bool Visible 
		{ 
			get 
			{ 
				if (!_visible) return false;
				return getBooleanFromParentIfNeeded(o => o.Visible, this.TreeNode.Parent); 
			} 
			set { _visible = value; } 
		}

		public bool Enabled 
		{ 
			get
			{
				if (!_enabled) return false;
				return getBooleanFromParentIfNeeded(o => o.Enabled, this.TreeNode.Parent);
			}
			set { _enabled = value; }
		}

		public string Hotspot { get; set; }

		public bool DebugDrawAnchor { get { return _animation.DebugDrawAnchor; } set { _animation.DebugDrawAnchor = value; } }

		public IBorderStyle Border { get { return _animation.Border; } set { _animation.Border = value; } }

		public bool CollidesWith(float x, float y)
		{
			ISquare boundingBox = BoundingBox;
			if (boundingBox == null)
				return false;
			IArea pixelPerfect = PixelPerfectHitTestArea;

			if (IgnoreViewport && _state != null) //todo: account for viewport scaling as well
			{
				x -= _state.Player.Character.Room.Viewport.X;
				y -= _state.Player.Character.Room.Viewport.Y;
			}

			if (pixelPerfect == null || !pixelPerfect.Enabled)
			{
				if (boundingBox.Contains(new AGSPoint (x, y)))
					return true;
			}
			else
			{
				if (pixelPerfect.IsInArea(new AGSPoint (x, y), boundingBox, ScaleX * Animation.Sprite.ScaleX,
					ScaleY * Animation.Sprite.ScaleY))
					return true;
			}
			return false;
		}

		public override string ToString ()
		{
			return Hotspot ?? base.ToString ();
		}

		#endregion

		private void refreshRoom()
		{
			_cachedRoom = new Lazy<IRoom> (getRoom, true);
		}

		private IRoom getRoom()
		{
			if (_state == null) return null;
			foreach (var room in _state.Rooms)
			{
				if (room.Objects.Contains(this)) return room;
			}
			return null;
		}

		private bool getBooleanFromParentIfNeeded(Predicate<IObject> getCurrent, IObject obj)
		{
			if (obj == null) return true;
			if (obj.TreeNode.Parent == null) return getCurrent(obj);
			if (!getCurrent(obj)) return false;
			return getBooleanFromParentIfNeeded(getCurrent, obj.TreeNode.Parent);
		}
	}
}

