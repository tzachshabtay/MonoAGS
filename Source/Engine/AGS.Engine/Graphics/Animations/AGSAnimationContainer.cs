using AGS.API;
using System.Threading.Tasks;

namespace AGS.Engine
{
	public class AGSAnimationContainer : AGSComponent, IAnimationContainer
	{
		private IGraphicsFactory _factory;
        private IScale _scale;
        private float _initialWidth, _initialHeight;

		/// <summary>
		/// Initializes a new instance of the <see cref="AGS.Engine.AGSAnimationContainer"/> class.
		/// Width and height will be set based on the first animation frame (or single image) used.
		/// </summary>
		/// <param name="sprite">Sprite.</param>
		/// <param name="factory">Factory.</param>
		public AGSAnimationContainer(IGraphicsFactory factory)
		{
            this._factory = factory;
            OnAnimationStarted = new AGSEvent<AGSEventArgs>();
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="AGS.Engine.AGSAnimationContainer"/> class.
		/// Initialized with preset width and height. 
		/// </summary>
		/// <param name="width">Width.</param>
		/// <param name="height">Height.</param>
		/// <param name="sprite">Sprite.</param>
		/// <param name="factory">Factory.</param>
		public AGSAnimationContainer(float width, float height, IGraphicsFactory factory) : 
			this(factory)
		{
            _initialWidth = width;
            _initialHeight = height;
		}

		public IAnimation Animation { get; private set; }

        public IEvent<AGSEventArgs> OnAnimationStarted { get; private set; }

		public bool DebugDrawAnchor { get; set; }

		public IBorderStyle Border { get; set; }

		public override void Init(IEntity entity)
        {
            base.Init(entity);
            _scale = entity.GetComponent<IScaleComponent>();            
        }        

        public void StartAnimation(IAnimation animation)
		{
			if (_scale.Width == 0f && animation.Frames.Count > 0) 
			{
				_scale.ResetBaseSize(animation.Frames [0].Sprite.Width, animation.Frames [0].Sprite.Height);
			}
			IAnimation currentAnimation = Animation;
			if (currentAnimation != null) 
			{
				currentAnimation.State.OnAnimationCompleted.TrySetResult (new AnimationCompletedEventArgs (false));
			}
			Animation = animation;
            OnAnimationStarted.Invoke(this, new AGSEventArgs());
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
	}
}

