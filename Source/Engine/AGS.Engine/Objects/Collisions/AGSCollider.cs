using AGS.API;

namespace AGS.Engine
{
	public class AGSCollider : AGSComponent, ICollider
	{
		private IGameState _state;
		private IDrawableInfo _drawableInfo;
		private IAnimationContainer _obj;
        private IScale _scale;
        private IPixelPerfectCollidable _pixelPerfect;

		public AGSCollider(IGameState state)
		{
			_state = state;
		}

		public override void Init(IEntity entity)
		{
			base.Init(entity);
			_drawableInfo = entity.GetComponent<IDrawableInfo>();
			_obj = entity.GetComponent<IAnimationContainer>();
            _scale = entity.GetComponent<IScaleComponent>();
            _pixelPerfect = entity.GetComponent<IPixelPerfectComponent>();
		}

		public AGSSquare BoundingBox { get; set; }

		public PointF? CenterPoint
		{
			get
			{
				float minX = BoundingBox.MinX;
				float minY = BoundingBox.MinY;
				float offsetX = _pixelPerfect.PixelPerfectHitTestArea == null ? (BoundingBox.MaxX - BoundingBox.MinX) / 2f :
                    _pixelPerfect.PixelPerfectHitTestArea.Mask.MinX + (_pixelPerfect.PixelPerfectHitTestArea.Mask.MaxX - _pixelPerfect.PixelPerfectHitTestArea.Mask.MinX) / 2f;
				float offsetY = _pixelPerfect.PixelPerfectHitTestArea == null ? (BoundingBox.MaxY - BoundingBox.MinY) / 2f :
                    _pixelPerfect.PixelPerfectHitTestArea.Mask.MinY + (_pixelPerfect.PixelPerfectHitTestArea.Mask.MaxY - _pixelPerfect.PixelPerfectHitTestArea.Mask.MinY) / 2f;

				return new PointF (minX + offsetX, minY + offsetY);
			}
		}

		public bool CollidesWith(float x, float y)
		{
			AGSSquare boundingBox = BoundingBox;
			IArea pixelPerfect = _pixelPerfect.PixelPerfectHitTestArea;

			if (_drawableInfo.IgnoreViewport && _state != null) 
			{
				var viewport = _state.Room.Viewport;
				//todo: Support viewport rotation (+ ignore scaling areas = false?)
				x = (x - viewport.X) * viewport.ScaleX;
				y = (y - viewport.Y) * viewport.ScaleY;
			}

			if (pixelPerfect == null || !pixelPerfect.Enabled)
			{
				if (boundingBox.Contains(new PointF (x, y)))
					return true;
			}
			else
			{
				if (pixelPerfect.IsInArea(new PointF (x, y), boundingBox, _scale.ScaleX * _obj.Animation.Sprite.ScaleX,
					_scale.ScaleY * _obj.Animation.Sprite.ScaleY))
					return true;
			}
			return false;
		}
	}
}

