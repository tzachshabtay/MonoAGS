using System;
using OpenTK;
using AGS.API;

namespace AGS.Engine
{
	public class GLViewportMatrix : IGLViewportMatrix
	{
		private Matrix4 _lastMatrix;
		private float _lastScaleX, _lastScaleY, _lastX, _lastY, _lastParallaxSpeedX, _lastParallaxSpeedY;

		public GLViewportMatrix()
		{
			_lastScaleX = 1f;
			_lastScaleY = 1f;
			_lastParallaxSpeedX = 1f;
			_lastParallaxSpeedY = 1f;
			buildMatrix();
		}

		#region IGLViewportMatrix implementation

		public Matrix4 GetMatrix(IViewport viewport, PointF parallaxSpeed)
		{
			if (viewport.X == _lastX && viewport.Y == _lastY &&
			    viewport.ScaleX == _lastScaleX && viewport.Y == _lastScaleY &&
				parallaxSpeed.X == _lastParallaxSpeedX && parallaxSpeed.Y == _lastParallaxSpeedY)
			{
				return _lastMatrix;
			}

			_lastX = viewport.X;
			_lastY = viewport.Y;
			_lastScaleX = viewport.ScaleX;
			_lastScaleY = viewport.ScaleY;
			_lastParallaxSpeedX = parallaxSpeed.X;
			_lastParallaxSpeedY = parallaxSpeed.Y;
			buildMatrix();

			return _lastMatrix;
		}

		#endregion

		private void buildMatrix()
		{
			_lastMatrix = 
				Matrix4.CreateTranslation(new Vector3(-_lastX * _lastParallaxSpeedX, -_lastY * _lastParallaxSpeedY, 0f)) * 
				Matrix4.CreateScale(_lastScaleX, _lastScaleY, 1f);
		}
	}
}

