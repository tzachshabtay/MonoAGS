using System;
using AGS.API;
using System.Collections.Generic;

namespace AGS.Engine
{
	public class RoomTransitionBoxOut : IRoomTransition
	{
		private readonly float _timeInSeconds, _screenWidth, _screenHeight, _screenHalfWidth, _screenHalfHeight;
		private readonly Func<float, float> _easingBoxOut, _easingBoxIn;
		private readonly QuadVectors _screenVectors;

		private Tween _tweenWidth;
		private Action _visitTweenWidth, _visitTweenHeight;
		private float _boxWidth, _boxHeight;
		private bool _isBoxIn;

		public RoomTransitionBoxOut(float timeInSeconds = 1f, Func<float, float> easingBoxOut = null, 
			Func<float, float> easingBoxIn = null, IGame game = null)
		{
			_timeInSeconds = timeInSeconds / 2f;
			_easingBoxOut = easingBoxOut ?? Ease.Linear;
			_easingBoxIn = easingBoxIn ?? Ease.Linear;
			game = game ?? AGSGame.Game;
			_screenVectors = new QuadVectors (game);
			_screenWidth = game.VirtualResolution.Width;
			_screenHeight = game.VirtualResolution.Height;
			_screenHalfWidth = _screenWidth / 2f;
			_screenHalfHeight = _screenHeight / 2f;
		}

		#region IRoomTransition implementation

		public bool RenderBeforeLeavingRoom(List<IObject> displayList, Action<IObject> renderObj)
		{
			_tweenWidth = Tween.RunWithExternalVisit(0f, _screenWidth, f => _boxWidth = f, _timeInSeconds, _easingBoxOut, out _visitTweenWidth);
			Tween.RunWithExternalVisit(0f, _screenHeight, f => _boxHeight = f, _timeInSeconds, _easingBoxOut, out _visitTweenHeight);
			return false;
		}

		public bool RenderTransition(IFrameBuffer from, IFrameBuffer to)
		{
			if (_tweenWidth.Task.IsCompleted)
			{
				if (_isBoxIn)
				{
					_isBoxIn = false;
					return false;
				}
				_isBoxIn = true;
				_tweenWidth = Tween.RunWithExternalVisit(0f, _screenWidth, f => _boxWidth = f, _timeInSeconds, _easingBoxIn, out _visitTweenWidth);
				Tween.RunWithExternalVisit(0f, _screenHeight, f => _boxHeight = f, _timeInSeconds, _easingBoxIn, out _visitTweenHeight);
			}

			_visitTweenWidth();
			_visitTweenHeight();

			if (_isBoxIn)
			{
				_screenVectors.Render(to.Texture);

				float x = _screenHalfWidth - _boxWidth / 2;
				float y = _screenHalfHeight - _boxHeight / 2;
				QuadVectors left = new QuadVectors (0f, 0f, x, _screenHeight);
				QuadVectors right = new QuadVectors (_screenWidth - x, 0f, x, _screenHeight);
				QuadVectors top = new QuadVectors (0f, 0f, _screenWidth, y);
				QuadVectors bottom = new QuadVectors (0f, _screenHeight - y, _screenWidth, y);
				renderBlackQuads(left, right, top, bottom);
			}
			else
			{
				_screenVectors.Render(from.Texture);
				QuadVectors quad = new QuadVectors (_screenHalfWidth - _boxWidth / 2, _screenHalfHeight - _boxHeight / 2, _boxWidth, _boxHeight);
				renderBlackQuads(quad);
			}
			return true;
		}

		public bool RenderAfterEnteringRoom(List<IObject> displayList, Action<IObject> renderObj)
		{
			return false;
		}

		#endregion

		private void renderBlackQuads(params QuadVectors[] quads)
		{
			foreach (var quad in quads)
			{
				quad.Render(0, 0f,0f,0f);
			}
		}
	}
}

