using System.Diagnostics;
using AGS.API;

namespace AGS.Engine
{
	public class AGSCollider : AGSComponent, IColliderComponent
	{
		private IGameState _state;
		private IDrawableInfoComponent _drawableInfo;
		private IImageComponent _obj;
        private IScale _scale;
        private IPixelPerfectComponent _pixelPerfect;
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
            entity.Bind<IDrawableInfoComponent>(c => _drawableInfo = c, _ => _drawableInfo = null);
            entity.Bind<IImageComponent>(c => _obj = c, _ => _obj = null);
            entity.Bind<IScaleComponent>(c => _scale = c, _ => _scale = null);
            entity.Bind<IPixelPerfectComponent>(c => _pixelPerfect = c, _ => _pixelPerfect = null);
            entity.Bind<IBoundingBoxComponent>(c => _boundingBox = c, _ => _boundingBox = null);
		}

        [Property(Browsable = false)]
		public PointF? CenterPoint
		{
			get
			{
                var boundingBoxesComponent = _boundingBox;
                if (boundingBoxesComponent == null) return null;
                var boundingBox = boundingBoxesComponent.HitTestBoundingBox;
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

        public bool CollidesWith(float x, float y, IViewport viewport)
		{
			var boundingBoxesComponent = _boundingBox;
            if (boundingBoxesComponent == null) return false;
            AGSBoundingBox boundingBox = boundingBoxesComponent.HitTestBoundingBox;
            var pixelPerfectComponent = _pixelPerfect;
            IArea pixelPerfect = pixelPerfectComponent == null || !pixelPerfectComponent.IsPixelPerfect ? null : pixelPerfectComponent.PixelPerfectHitTestArea;

            if (!(_drawableInfo?.IgnoreViewport ?? false))
			{
                var matrix = viewport.GetMatrix(_drawableInfo.RenderLayer);
                matrix.Invert();
                Vector3 xyz = new Vector3(x, y, 0f);
                Vector3.Transform(ref xyz, ref matrix, out xyz);
                //todo: (support ignore scaling areas = false?)
                x = xyz.X;
                y = xyz.Y;
			}

			if (pixelPerfect == null || !pixelPerfect.Enabled)
			{
                if (!boundingBox.IsValid) return false;
                if (boundingBox.Contains(new Vector2 (x, y)))
					return true;
			}
			else
			{
                var obj = _obj;
                float objScaleX = obj == null ? 1f : obj.CurrentSprite.ScaleX;
                float objScaleY = obj == null ? 1f : obj.CurrentSprite.ScaleY;
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