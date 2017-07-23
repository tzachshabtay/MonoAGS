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
        private IEntity _entity;
        private AGSBoundingBox _emptyBox = default(AGSBoundingBox);

		public AGSCollider(IGameState state)
		{
			_state = state;
		}

		public override void Init(IEntity entity)
		{
			base.Init(entity);
            _entity = entity;
            entity.Bind<IDrawableInfo>(c => _drawableInfo = c, _ => _drawableInfo = null);
            entity.Bind<IAnimationContainer>(c => _obj = c, _ => _obj = null);
            entity.Bind<IScaleComponent>(c => _scale = c, _ => _scale = null);
            entity.Bind<IPixelPerfectComponent>(c => _pixelPerfect = c, _ => _pixelPerfect = null);
		}

		public AGSBoundingBox BoundingBox { get; set; }

		public PointF? CenterPoint
		{
			get
			{
                if (BoundingBox.Equals(_emptyBox)) return null;
                var pixelPerfectComponent = _pixelPerfect;
                var pixelPerfect = pixelPerfectComponent == null ? null : pixelPerfectComponent.PixelPerfectHitTestArea;
				float minX = BoundingBox.MinX;
				float minY = BoundingBox.MinY;
				float offsetX = pixelPerfect == null ? (BoundingBox.MaxX - BoundingBox.MinX) / 2f :
                    pixelPerfect.Mask.MinX + (pixelPerfect.Mask.MaxX - pixelPerfect.Mask.MinX) / 2f;
				float offsetY = pixelPerfect == null ? (BoundingBox.MaxY - BoundingBox.MinY) / 2f :
                    pixelPerfect.Mask.MinY + (pixelPerfect.Mask.MaxY - pixelPerfect.Mask.MinY) / 2f;

				return new PointF (minX + offsetX, minY + offsetY);
			}
		}

		public bool CollidesWith(float x, float y)
		{
			AGSBoundingBox boundingBox = BoundingBox;
            if (boundingBox.Equals(_emptyBox)) return false;
            var pixelPerfectComponent = _pixelPerfect;
			IArea pixelPerfect = pixelPerfectComponent == null ? null : pixelPerfectComponent.PixelPerfectHitTestArea;

            var drawableInfo = _drawableInfo;
            if (drawableInfo != null && drawableInfo.IgnoreViewport && _state != null) 
			{
				var viewport = _state.Room.Viewport;
				//todo: Support viewport rotation (+ ignore scaling areas = false?)
				x = (x - viewport.X) * viewport.ScaleX;
				y = (y - viewport.Y) * viewport.ScaleY;
			}

			if (pixelPerfect == null || !pixelPerfect.Enabled)
			{
                if (boundingBox.Contains(new Vector2 (x, y)))
					return true;
			}
			else
			{
                var obj = _obj;
                float objScaleX = obj == null ? 1f : obj.Animation.Sprite.ScaleX;
                float objScaleY = obj == null ? 1f : obj.Animation.Sprite.ScaleY;
                var scale = _scale;
                float scaleX = scale == null ? 1f : scale.ScaleX;
                float scaleY = scale == null ? 1f : scale.ScaleY;
				if (pixelPerfect.IsInArea(new PointF (x, y), boundingBox, scaleX * objScaleX, scaleY * objScaleY))
					return true;
			}
			return false;
		}
	}
}

