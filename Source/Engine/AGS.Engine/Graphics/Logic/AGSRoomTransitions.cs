using System;
using AGS.API;

namespace AGS.Engine
{
	public class AGSRoomTransitions : IAGSRoomTransitions
	{
		private IRoomTransition _transition, _oneTimeTransition;
		private RoomTransitionState _state;

		public AGSRoomTransitions(IEvent<AGSEventArgs> onStateChanged)
		{
			OnStateChanged = onStateChanged;
			//starting with preparing transition so that the before fade in event will be called for the first room in the game
			_state = RoomTransitionState.PreparingTransition; 
		}

		#region IRoomTransitions implementation

		public IRoomTransition Transition 
		{ 
			get { return _oneTimeTransition ?? _transition; }
			set { _transition = value; }
		}

		public void SetOneTimeNextTransition(IRoomTransition transition)
		{
			_oneTimeTransition = transition;
		}

		public RoomTransitionState State 
		{
			get { return _state; }
			set 
			{
				if (value == _state) return;
				_state = value;
				OnStateChanged.Invoke(this, new AGSEventArgs ());
			}
		}

		public IEvent<AGSEventArgs> OnStateChanged { get; private set; }

		#endregion

		public static IRoomTransition Instant()
		{
			return new RoomTransitionInstant ();
		}

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
            return new RoomTransitionDissolve (AGSGame.GLUtils, Hooks.GraphicsBackend, timeInSeconds, ease);
		}

		public static IRoomTransition BoxOut(float timeInSeconds = 1f, Func<float, float> easeBoxOut = null,
			Func<float, float> easeBoxIn = null)
		{
			return new RoomTransitionBoxOut (AGSGame.GLUtils, timeInSeconds, easeBoxOut, easeBoxIn);
		}
	}
}

