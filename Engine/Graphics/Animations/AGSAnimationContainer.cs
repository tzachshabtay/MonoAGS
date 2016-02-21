using System;
using AGS.API;
using System.Drawing;
using System.Threading.Tasks;

namespace AGS.Engine
{
	public class AGSAnimationContainer : IAnimationContainer
	{
		private float _initialWidth, _initialHeight;
		private ISprite _sprite;
		private IGraphicsFactory _factory;
		private bool _pixelPerfect;

		/// <summary>
		/// Initializes a new instance of the <see cref="AGS.Engine.AGSAnimationContainer"/> class.
		/// Width and height will be set based on the first animation frame (or single image) used.
		/// </summary>
		/// <param name="sprite">Sprite.</param>
		/// <param name="factory">Factory.</param>
		public AGSAnimationContainer(ISprite sprite, IGraphicsFactory factory)
		{
			this._factory = factory;
			this._sprite = sprite;
			Anchor = new AGSPoint (0.5f, 0f);
			Visible = true;

			ScaleX = 1;
			ScaleY = 1;

			Tint = Color.White;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="AGS.Engine.AGSAnimationContainer"/> class.
		/// Initialized with preset width and height. 
		/// </summary>
		/// <param name="width">Width.</param>
		/// <param name="height">Height.</param>
		/// <param name="sprite">Sprite.</param>
		/// <param name="factory">Factory.</param>
		public AGSAnimationContainer(float width, float height, ISprite sprite, IGraphicsFactory factory) : 
			this(sprite, factory)
		{
			initScale (width, height);
		}

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
			Anchor = new AGSPoint (-Anchor.X, Anchor.Y);
		}

		public void FlipVertically()
		{
			ScaleBy(ScaleX, -ScaleY);
			Anchor = new AGSPoint (Anchor.X, -Anchor.Y);
		}

		public ISprite Clone()
		{
			ISprite sprite = Animation == null ? _sprite : Animation.Sprite;
			return sprite.Clone();
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

		public float X { get { return _sprite.X; } set { _sprite.X = value; } }
		public float Y { get { return _sprite.Y; } set { _sprite.Y = value; } }
		public float Z { get { return _sprite.Z; } set { _sprite.Z = value; } }

		public IAnimation Animation { get; private set; }

		public bool Visible { get; set; }

		public bool DebugDrawAnchor { get; set; }

		public IBorderStyle Border { get; set; }

		public IImage Image 
		{ 
			get 
			{ 
				if (Animation == null || Animation.Sprite == null) return null;
				return Animation.Sprite.Image; 
			}
			set 
			{ 
				AGSSingleFrameAnimation animation = new AGSSingleFrameAnimation (value, _factory);
				initScale(value.Width, value.Height);
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
			PixelPerfect(_pixelPerfect);
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

		public IArea PixelPerfectHitTestArea  
		{ 
			get 
			{ 
				if (Animation == null || Animation.Sprite == null) return null;
				return Animation.Sprite.PixelPerfectHitTestArea; 
			} 
		}
		
		public void PixelPerfect(bool pixelPerfect)
		{
			_pixelPerfect = pixelPerfect;
			if (Animation == null || Animation.Frames == null) return;
			foreach (var frame in Animation.Frames)
			{
				frame.Sprite.PixelPerfect(pixelPerfect);
			}
		}

		private void initScale(float width, float height)
		{
			Width = width * ScaleX;
			Height = height * ScaleY;
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
	}
}

