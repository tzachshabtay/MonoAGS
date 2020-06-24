﻿using AGS.API;

namespace AGS.Engine
{
	public class AGSCollider : AGSComponent, IColliderComponent
	{
	    private IDrawableInfoComponent _drawableInfo;
		private IImageComponent _obj;
        private IScale _scale;
        private IPixelPerfectComponent _pixelPerfect;
        private IBoundingBoxComponent _boundingBox;
        private ICropSelfComponent _crop;

		public override void Init()
		{
			base.Init();
            Entity.Bind<IDrawableInfoComponent>(c => _drawableInfo = c, _ => _drawableInfo = null);
            Entity.Bind<IImageComponent>(c => _obj = c, _ => _obj = null);
            Entity.Bind<IScaleComponent>(c => _scale = c, _ => _scale = null);
            Entity.Bind<IPixelPerfectComponent>(c => _pixelPerfect = c, _ => _pixelPerfect = null);
            Entity.Bind<IBoundingBoxComponent>(c => _boundingBox = c, _ => _boundingBox = null);
            Entity.Bind<ICropSelfComponent>(c => _crop = c, _ => _crop = null);
		}

        [Property(Browsable = false)]
		public PointF? CenterPoint
		{
			get
			{
                var boundingBoxesComponent = _boundingBox;
                if (boundingBoxesComponent == null) return null;
                var boundingBox = boundingBoxesComponent.WorldBoundingBox;
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
            if (_crop?.IsGuaranteedToFullyCrop() ?? false)
            {
                //This should not be needed as the bounding box should be cropped anyway.
                //However, for a performance optimization if an entity is guaranteed to be fully cropped (like items in the bottom of a long listbox) the bounding box is not calculated, so we need to take it into account here.
                return false;
            }
            AGSBoundingBox boundingBox = boundingBoxesComponent.WorldBoundingBox;
            var pixelPerfectComponent = _pixelPerfect;
            IArea pixelPerfect = pixelPerfectComponent == null || !pixelPerfectComponent.IsPixelPerfect ? null : pixelPerfectComponent.PixelPerfectHitTestArea;

		    var drawable = _drawableInfo;
            if (drawable != null && !drawable.IgnoreViewport)
			{
                var matrix = viewport.GetMatrix(drawable.RenderLayer);
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
