using System;
using AGS.API;
using System.Collections.Generic;

namespace AGS.Engine
{
	public class RoomTransitionSlide : IRoomTransition
	{
		private readonly bool _slideIn;
		private readonly float _targetX, _targetY;
		private readonly Func<float, float> _easingX, _easingY;
		private readonly float _timeInSeconds;
		private readonly float _width, _height;
		private readonly QuadVectors _screenVectors;

		private Tween _tweenX, _tweenY;
		private Action _visitTweenX, _visitTweenY;
		private float _x, _y;

		public RoomTransitionSlide(bool slideIn = false, float? x = null, float y = 0f, 
			float timeInSeconds = 1f, Func<float, float> easingX = null,
			Func<float, float> easingY = null, IGame game = null)
		{
			game = game ?? AGSGame.Game;
			_timeInSeconds = timeInSeconds;
			_slideIn = slideIn;
			_easingX = easingX ?? Ease.QuadIn;
			_easingY = easingY ?? Ease.QuadIn;
			_targetX = x == null ? game.VirtualResolution.Width : x.Value;
			_targetY = y;
			_width = game.VirtualResolution.Width;
			_height = game.VirtualResolution.Height;
			_screenVectors = new QuadVectors (game);
		}

		#region IRoomTransition implementation

		public bool RenderBeforeLeavingRoom(List<IObject> displayList, Action<IObject> renderObj)
		{
			_x = _slideIn ? _targetX : 0f;
			_y = _slideIn ? _targetY : 0f;
			float toX = _slideIn ? 0f : _targetX;
			float toY = _slideIn ? 0f : _targetY;
			_tweenX = Tween.RunWithExternalVisit(_x, toX, b => _x = b, _timeInSeconds, _easingX, out _visitTweenX);
			_tweenY = Tween.RunWithExternalVisit(_y, toY, b => _y = b, _timeInSeconds, _easingY, out _visitTweenY);
			return false;
		}

		public bool RenderTransition(IFrameBuffer from, IFrameBuffer to)
		{
			if (_tweenX.Task.IsCompleted && _tweenY.Task.IsCompleted)
			{
				return false;
			}
			if (!_tweenX.Task.IsCompleted) _visitTweenX();
			if (!_tweenY.Task.IsCompleted) _visitTweenY();

			var quad = new QuadVectors (_x, _y, _width, _height);
			if (_slideIn)
			{
				_screenVectors.Render(from.Texture);
				quad.Render(to.Texture);
			}
			else
			{
				_screenVectors.Render(to.Texture);
				quad.Render(from.Texture);
			}
			return true;
		}

		public bool RenderAfterEnteringRoom(List<IObject> displayList, Action<IObject> renderObj)
		{
			return false;
		}

		#endregion
	}
}

