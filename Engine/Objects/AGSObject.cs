using System;
using API;
using System.Threading.Tasks;
using System.Drawing;

namespace Engine
{
	public class AGSObject : IObject
	{
		private float _initialWidth, _initialHeight;
		private IObject _parent;
		private ISprite _sprite;

		/// <summary>
		/// Initializes a new instance of the <see cref="Engine.AGSObject"/> class.
		/// Width and height will be set based on the first animation frame (or single image) used.
		/// </summary>
		public AGSObject (ISprite sprite)
		{
			this._sprite = sprite;
			Location = new AGSLocation ();
			Anchor = new AGSPoint (0.5f, 0f);
			Visible = true;
			Enabled = true;

			ScaleX = 1;
			ScaleY = 1;

			Tint = Color.White;

			RenderLayer = AGSLayers.Foreground;
			TreeNode = new AGSTreeNode<IObject> (this);
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="Engine.AGSObject"/> class.
		/// Initialized with preset width and height. 
		/// </summary>
		/// <param name="width">Width.</param>
		/// <param name="height">Height.</param>
		public AGSObject(float width, float height, ISprite sprite) : this(sprite)
		{
			initScale (width, height);
		}

		private void initScale(float width, float height)
		{
			Width = width;
			Height = height;
			_initialWidth = width;
			_initialHeight = height;
		}

		private void validateScaleInitialized()
		{
			if (_initialWidth == 0f) 
			{
				throw new InvalidOperationException (
					"Initial size was not set. Either assign an animation/image to the object, or use the appropriate constructor.");
			}
		}

		#region IObject implementation

		public ITreeNode<IObject> TreeNode { get; private set; }

		public void ResetScale ()
		{
			Width = _initialWidth;
			Height = _initialHeight;
			ScaleX = 1;
			ScaleY = 1;
		}

		public void ScaleBy (float scaleX, float scaleY)
		{
			validateScaleInitialized ();
			ScaleX = scaleX;
			ScaleY = scaleY;
			Width = _initialWidth * ScaleX;
			Height = _initialHeight * ScaleY;
		}

		public void ScaleTo (float width, float height)
		{
			validateScaleInitialized ();
			Width = width;
			Height = height;
			ScaleX = Width / _initialWidth;
			ScaleY = Height / _initialHeight;
		}

		public void FlipHorizontally()
		{
			ScaleBy(-ScaleX, ScaleY);
		}

		public void FlipVertically()
		{
			ScaleBy(ScaleX, -ScaleY);
		}

		public ILocation Location { get { return _sprite.Location; } set { _sprite.Location = value; } }

		public float Height { get; private set; }

		public float Width { get; private set; }

		public float ScaleX { get; private set; }

		public float ScaleY { get; private set; }

		public float Angle { get { return _sprite.Angle; } set { _sprite.Angle = value; } }

		public byte Opacity { get { return _sprite.Opacity; } set { _sprite.Opacity = value; } }

		public Color Tint { get { return _sprite.Tint; } set { _sprite.Tint = value; } }

		public IPoint Anchor { get { return _sprite.Anchor; } set { _sprite.Anchor = value; } }

		public ISquare BoundingBox { get; set; }

		public float X { get { return _sprite.X; } set { _sprite.X = value; } }
		public float Y { get { return _sprite.Y; } set { _sprite.Y = value; } }
		public float Z { get { return _sprite.Z; } set { _sprite.Z = value; } }

		public IRenderLayer RenderLayer { get; set; }

		public bool IgnoreViewport { get; set; }

		public IImage Image 
		{ 
			get { return Animation.Sprite.Image; }
			set 
			{ 
				AGSSingleFrameAnimation animation = new AGSSingleFrameAnimation (value);
				StartAnimation (animation);
			}
		}

		public IImageRenderer CustomRenderer 
		{ 
			get { return _sprite.CustomRenderer; } 
			set { _sprite.CustomRenderer = value; } 
		}

		public void StartAnimation(IAnimation animation)
		{
			if (_initialWidth == 0f && animation.Frames.Count > 0) 
			{
				initScale (animation.Frames [0].Sprite.Width, animation.Frames [0].Sprite.Height);
			}
			IAnimation currentAnimation = Animation;
			if (currentAnimation != null) 
			{
				currentAnimation.State.OnAnimationCompleted.TrySetResult (new AnimationCompletedEventArgs (false));
			}
			Animation = animation;
		}

		public AnimationCompletedEventArgs Animate (IAnimation animation)
		{
			var task = Task.Run (async () => await AnimateAsync (animation));
			return task.Result;
		}

		public async Task<AnimationCompletedEventArgs> AnimateAsync (IAnimation animation)
		{
			StartAnimation (animation);
			return await animation.State.OnAnimationCompleted.Task;
		}

		public IRoom Room { get; set; }

		public IAnimation Animation { get; private set; }

		public IInteractions Interactions { get; private set; }

		public bool Visible { get; set; }

		public bool Enabled { get; set; }

		public string Hotspot { get; set; }

		public bool DebugDrawAnchor { get; set; }

		public IBorderStyle Border { get; set; }

		public override string ToString ()
		{
			return Hotspot ?? base.ToString ();
		}

		#endregion
	}
}

