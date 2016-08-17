using System.Threading.Tasks;

namespace AGS.API
{    
    public interface IAnimationContainer : IComponent
	{
		IAnimation Animation { get; }
		bool DebugDrawAnchor { get; set; }
        IEvent<AGSEventArgs> OnAnimationStarted { get; }

		IBorderStyle Border { get; set; }

		void StartAnimation(IAnimation animation);
		AnimationCompletedEventArgs Animate(IAnimation animation);
		Task<AnimationCompletedEventArgs> AnimateAsync(IAnimation animation);		
	}
}

