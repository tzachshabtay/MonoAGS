using AGS.API;
using System.ComponentModel;
using System.Threading.Tasks;

namespace AGS.Engine
{
	public class AGSAnimationComponent : AGSComponent, IAnimationComponent, ISpriteProvider
	{
        private IScale _scale;
        private ISpriteRenderComponent _spriteRender;

		public AGSAnimationComponent()
		{
            OnAnimationStarted = new AGSEvent();
		}		

		public IAnimation Animation { get; private set; }

        public IBlockingEvent OnAnimationStarted { get; private set; }

		public override void Init(IEntity entity)
        {
            base.Init(entity);
            entity.Bind<IScaleComponent>(c => _scale = c, c => _scale = null);
            entity.Bind<ISpriteRenderComponent>(c => _spriteRender = c, c => _spriteRender = null);
        }

        public void StartAnimation(IAnimation animation)
		{
            var scale = _scale;
            if (scale?.Width == 0f && animation.Frames.Count > 0) 
			{
                scale.BaseSize = new SizeF(animation.Frames[0].Sprite.Width, animation.Frames[0].Sprite.Height);
			}
			IAnimation currentAnimation = Animation;
            if (currentAnimation != null)
            {
                currentAnimation.State.PropertyChanged -= OnAnimationStatePropertyChanged;
                currentAnimation.State.OnAnimationCompleted.TrySetResult(new AnimationCompletedEventArgs(false));
            }

            Animation = animation;
            animation.State.PropertyChanged += OnAnimationStatePropertyChanged;
            OnAnimationStarted.Invoke();
            if (_spriteRender != null)
                _spriteRender.SpriteProvider = this;
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

        [Property(Browsable = false)]
        public ISprite Sprite { get => Animation?.Sprite; }

        private void OnAnimationStatePropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName != nameof(IAnimationState.CurrentFrame))
                return;
            // resend property changed event to notify that ISpriteProvider.Sprite has new value
            OnPropertyChanged(nameof(Sprite));
        }

    }
}
