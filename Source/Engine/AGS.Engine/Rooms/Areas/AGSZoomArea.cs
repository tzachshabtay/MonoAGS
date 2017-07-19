using System;
using AGS.API;

namespace AGS.Engine
{
    public class AGSZoomArea : AGSComponent, IZoomArea
    {
        private IAreaComponent _area;

        public AGSZoomArea()
        {
            ZoomCamera = true;
        }

        public override void Init(IEntity entity)
        {
            base.Init(entity);
            entity.Bind<IAreaComponent>(c => _area = c, _ => _area = null);
        }

        public float MinZoom { get; set; }

        public float MaxZoom { get; set; }

        public bool ZoomCamera { get; set; }

        public float GetZoom(float value)
        {
            return MathUtils.Lerp(_area.Mask.MinY, MinZoom, _area.Mask.MaxY, MaxZoom, value);
        }
    }
}
