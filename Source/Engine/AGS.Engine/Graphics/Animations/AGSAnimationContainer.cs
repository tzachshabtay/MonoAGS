using AGS.API;
using System.Threading.Tasks;

namespace AGS.Engine
{
	public class AGSAnimationContainer : AGSComponent, IAnimationContainer
	{
        private IScale _scale;

		public AGSAnimationContainer()
		{
            OnAnimationStarted = new AGSEvent();
		}		

		public IAnimation Animation { get; private set; }

        public IBlockingEvent OnAnimationStarted { get; private set; }

		public bool DebugDrawAnchor { get; set; }

		public IBorderStyle Border { get; set; }

		public override void Init(IEntity entity)
        {
            base.Init(entity);
            entity.Bind<IScaleComponent>(c => _scale = c, c => _scale = null);
        }        

        public void StartAnimation(IAnimation animation)
		{
            var scale = _scale;
            if (scale?.Width == 0f && animation.Frames.Count > 0) 
			{
                scale.BaseSize = new SizeF(animation.Frames[0].Sprite.Width, animation.Frames[0].Sprite.Height);
			}
			IAnimation currentAnimation = Animation;
			currentAnimation?.State.OnAnimationCompleted.TrySetResult (new AnimationCompletedEventArgs (false));
			
			Animation = animation;
            OnAnimationStarted.Invoke();
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

