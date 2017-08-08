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
        private IBoundingBoxComponent _boundingBox;

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
            entity.Bind<IBoundingBoxComponent>(c => _boundingBox = c, _ => _boundingBox = null);
		}

		public PointF? CenterPoint
		{
			get
			{
                var boundingBoxesComponent = _boundingBox;
                if (boundingBoxesComponent == null) return null;
                var boundingBoxes = boundingBoxesComponent.GetBoundingBoxes();
                if (boundingBoxes == null) return null;
                var boundingBox = boundingBoxes.HitTestBox;
                var pixelPerfectComponent = _pixelPerfect;
                var pixelPerfect = pixelPerfectComponent == null ? null : pixelPerfectComponent.PixelPerfectHitTestArea;
				float minX = boundingBox.MinX;
				float minY = boundingBox.MinY;
				float offsetX = pixelPerfect == null ? (boundingBox.MaxX - boundingBox.MinX) / 2f :
                    pixelPerfect.Mask.MinX + (pixelPerfect.Mask.MaxX - pixelPerfect.Mask.MinX) / 2f;
				float offsetY = pixelPerfect == null ? (boundingBox.MaxY - boundingBox.MinY) / 2f :
                    pixelPerfect.Mask.MinY + (pixelPerfect.Mask.MaxY - pixelPerfect.Mask.MinY) / 2f;

				return new PointF (minX + offsetX, minY + offsetY);
			}
		}

		public bool CollidesWith(float x, float y)
		{
			var boundingBoxesComponent = _boundingBox;
            if (boundingBoxesComponent == null) return false;
			var boundingBoxes = boundingBoxesComponent.GetBoundingBoxes();
            if (boundingBoxes == null) return false;
            AGSBoundingBox boundingBox = boundingBoxes.HitTestBox;
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
                if (boundingBox.Equals(default(AGSBoundingBox))) return false;
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

