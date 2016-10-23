using System;
using AGS.API;

namespace AGS.Engine
{
	public class RoomTransitionCrossFade : IRoomTransition
	{
		private readonly float _timeInSeconds;
		private readonly Func<float, float> _easingFadeOut;
		private readonly QuadVectors _screenVectors;

		private Tween _tween;
		private Action _visitTween;
		private float _alpha;

		public RoomTransitionCrossFade(IGLUtils glUtils, float timeInSeconds = 1f, Func<float, float> easingFadeOut = null, 
			IGame game = null)
		{
			game = game ?? AGSGame.Game;
			_timeInSeconds = timeInSeconds / 2f;
			_easingFadeOut = easingFadeOut ?? Ease.Linear;
            _screenVectors = new QuadVectors (game, glUtils);
		}

		#region IRoomTransition implementation

		public bool RenderBeforeLeavingRoom(System.Collections.Generic.List<IObject> displayList, Action<IObject> renderObj)
		{
			_tween = Tween.RunWithExternalVisit(1f, 0f, b => _alpha = b, _timeInSeconds, _easingFadeOut, out _visitTween);
			return false;
		}

		public bool RenderTransition(IFrameBuffer from, IFrameBuffer to)
		{
			if (_tween.Task.IsCompleted)
			{
				return false;
			}
			_visitTween();
			_screenVectors.Render(to.Texture);
			_screenVectors.Render(from.Texture, a: _alpha);
			return true;
		}

		public bool RenderAfterEnteringRoom(System.Collections.Generic.List<IObject> displayList, Action<IObject> renderObj)
		{
			return false;
		}

		#endregion
	}
}

