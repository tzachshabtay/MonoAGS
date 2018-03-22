using AGS.API;

namespace AGS.Engine
{
	public class GLViewportMatrix : IGLViewportMatrix
	{
		private Matrix4 _lastMatrix;
		private float _lastScaleX, _lastScaleY, _lastX, _lastY, _lastParallaxSpeedX, _lastParallaxSpeedY, _lastRotation;
        private PointF _lastPivot;
        private readonly Size _virtualResolution;

        public GLViewportMatrix(IGameSettings settings)
		{
            _virtualResolution = settings.VirtualResolution;
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
                viewport.ScaleX == _lastScaleX && viewport.ScaleY == _lastScaleY &&
				parallaxSpeed.X == _lastParallaxSpeedX && parallaxSpeed.Y == _lastParallaxSpeedY &&
                viewport.Angle == _lastRotation && _lastPivot.Equals(viewport.Pivot))
			{
				return _lastMatrix;
			}

			_lastX = viewport.X;
			_lastY = viewport.Y;
			_lastScaleX = viewport.ScaleX;
			_lastScaleY = viewport.ScaleY;
			_lastParallaxSpeedX = parallaxSpeed.X;
			_lastParallaxSpeedY = parallaxSpeed.Y;
			_lastRotation = viewport.Angle;
            _lastPivot = viewport.Pivot;
			buildMatrix();

			return _lastMatrix;
		}

		#endregion

		private void buildMatrix()
		{
            var pivotOffsets = AGSModelMatrixComponent.GetPivotOffsets(_lastPivot, _virtualResolution.Width, _virtualResolution.Height);
            Matrix4 pivotMat = Matrix4.CreateTranslation(new Vector3(pivotOffsets.X, pivotOffsets.Y, 0f));
            var radians = MathUtils.DegreesToRadians(_lastRotation);
            _lastMatrix =
                Matrix4.CreateTranslation(new Vector3(-_lastX * _lastParallaxSpeedX, -_lastY * _lastParallaxSpeedY, 0f)) *
                Matrix4.CreateRotationZ(radians) *
                pivotMat *
                Matrix4.CreateScale(_lastScaleX, _lastScaleY, 1f);
		}
	}
}