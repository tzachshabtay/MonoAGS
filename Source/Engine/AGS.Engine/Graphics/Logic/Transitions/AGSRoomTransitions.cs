using System;
using AGS.API;

namespace AGS.Engine
{
	public class AGSRoomTransitions : IRoomTransitions
	{
		private IRoomTransition _transition, _oneTimeTransition;

		#region IRoomTransitions implementation

		public IRoomTransition Transition 
		{
            get => _oneTimeTransition ?? _transition;
            set { _transition = value; }
		}

		public void SetOneTimeNextTransition(IRoomTransition transition)
		{
			_oneTimeTransition = transition;
		}

        #endregion

        public static IRoomTransition Instant() => new RoomTransitionInstant();

        public static IRoomTransition Fade(float timeInSeconds = 1f, Func<float, float> easeFadeOut = null,
			Func<float, float> easeFadeIn = null)
		{
			return new RoomTransitionFade (AGSGame.GLUtils, timeInSeconds, easeFadeOut, easeFadeIn);
		}

		public static IRoomTransition Slide(bool slideIn = false, float? x = null, float y = 0f, float timeInSeconds = 1f,
			Func<float, float> easingX = null, Func<float, float> easingY = null)
		{
			return new RoomTransitionSlide (AGSGame.GLUtils, slideIn, x, y, timeInSeconds, easingX, easingY);
		}

		public static IRoomTransition CrossFade(float timeInSeconds = 1f, Func<float, float> easeFadeOut = null)
		{
			return new RoomTransitionCrossFade (AGSGame.GLUtils, timeInSeconds, easeFadeOut);
		}

		public static IRoomTransition Dissolve(float timeInSeconds = 1f, Func<float, float> ease = null)
		{
            return new RoomTransitionDissolve (AGSGame.GLUtils, timeInSeconds, ease);
		}

		public static IRoomTransition BoxOut(float timeInSeconds = 1f, Func<float, float> easeBoxOut = null,
			Func<float, float> easeBoxIn = null)
		{
			return new RoomTransitionBoxOut (AGSGame.GLUtils, timeInSeconds, easeBoxOut, easeBoxIn);
		}
	}
}