using System;
using OpenTK;
using AGS.API;

namespace AGS.Engine
{
	public class GLViewportMatrix : IGLViewportMatrix
	{
		private Matrix4 _lastMatrix;
		private float _lastScaleX, _lastScaleY, _lastX, _lastY;

		public GLViewportMatrix()
		{
			_lastScaleX = 1f;
			_lastScaleY = 1f;
			buildMatrix();
		}

		#region IGLViewportMatrix implementation

		public Matrix4 GetMatrix(IViewport viewport)
		{
			if (viewport.X == _lastX && viewport.Y == _lastY &&
			    viewport.ScaleX == _lastScaleX && viewport.Y == _lastScaleY)
			{
				return _lastMatrix;
			}

			_lastX = viewport.X;
			_lastY = viewport.Y;
			_lastScaleX = viewport.ScaleX;
			_lastScaleY = viewport.ScaleY;
			buildMatrix();

			return _lastMatrix;
		}

		#endregion

		private void buildMatrix()
		{
			_lastMatrix = Matrix4.CreateScale(_lastScaleX, _lastScaleY, 1f) *
				Matrix4.CreateTranslation(new Vector3(-_lastX, -_lastY, 0f));
		}
	}
}

