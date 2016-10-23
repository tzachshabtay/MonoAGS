using System;
using AGS.API;
using System.Collections.Generic;

namespace AGS.Engine
{
	public class RoomTransitionFade : IRoomTransition
	{
		private readonly float _timeInSeconds;
		private readonly Func<float, float> _easingFadeOut, _easingFadeIn;
		private readonly QuadVectors _screenVectors;

		private Tween _tween;
		private Action _visitTween;
		private float _black;
		private bool _isFadeIn;

		public RoomTransitionFade(IGLUtils glUtils, float timeInSeconds = 1f, Func<float, float> easingFadeOut = null, 
			Func<float, float> easingFadeIn = null, IGame game = null)
		{
			game = game ?? AGSGame.Game;
			_timeInSeconds = timeInSeconds / 2f;
			_easingFadeOut = easingFadeOut ?? Ease.Linear;
			_easingFadeIn = easingFadeIn ?? Ease.Linear;
            _screenVectors = new QuadVectors (game, glUtils);
		}

		#region IRoomTransition implementation

		public bool RenderBeforeLeavingRoom(List<IObject> displayList, Action<IObject> renderObj)
		{
			_tween = Tween.RunWithExternalVisit(1f, 0f, b => _black = b, _timeInSeconds, _easingFadeOut, out _visitTween);
			return false;
		}

		public bool RenderTransition(IFrameBuffer from, IFrameBuffer to)
		{
			if (_tween.Task.IsCompleted)
			{
				if (_isFadeIn)
				{
					_isFadeIn = false;
					return false;
				}
				_isFadeIn = true;
				_tween = Tween.RunWithExternalVisit(0f, 1f, b => _black = b, _timeInSeconds, _easingFadeIn, out _visitTween);
			}
			_visitTween();
			_screenVectors.Render(_isFadeIn ? to.Texture : from.Texture, _black, _black, _black);
			return true;
		}

		public bool RenderAfterEnteringRoom(List<IObject> displayList, Action<IObject> renderObj)
		{
			return false;
		}

		#endregion
	}
}

