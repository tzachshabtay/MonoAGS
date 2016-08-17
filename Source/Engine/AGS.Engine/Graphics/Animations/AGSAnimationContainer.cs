using AGS.API;
using System.Threading.Tasks;

namespace AGS.Engine
{
	public class AGSAnimationContainer : AGSComponent, IAnimationContainer
	{
        private IScale _scale;

		public AGSAnimationContainer()
		{
            OnAnimationStarted = new AGSEvent<AGSEventArgs>();
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

