using System.Threading.Tasks;

namespace AGS.API
{
    public interface IAnimationContainer : ISprite
	{
		IAnimation Animation { get; }
		bool DebugDrawAnchor { get; set; }

		IBorderStyle Border { get; set; }

		void StartAnimation(IAnimation animation);
		AnimationCompletedEventArgs Animate(IAnimation animation);
		Task<AnimationCompletedEventArgs> AnimateAsync(IAnimation animation);

		void ResetScale(float initialWidth, float initialHeight);
	}
}

