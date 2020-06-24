using AGS.API;
using System;

namespace AGS.Engine
{
    public class AGSArea : AGSEntity, IArea
    {
        private readonly IAreaComponent _areaComponent;

        public AGSArea(string id, Resolver resolver) : base(id, resolver)
        {
            _areaComponent = AddComponent<IAreaComponent>();

            InitComponents();
        }

        public string Name => ID;
        public bool AllowMultiple => false;
        public IEntity Entity => this;
        public Type RegistrationType => typeof(IArea);
        public void Init(IEntity entity, Type registrationType) { }

        public override string ToString() => $"{ID ?? ""} ({GetType().Name})";

        public bool Enabled
        {
            get => _areaComponent.Enabled;
            set => _areaComponent.Enabled = value;
        }

        public IMask Mask
        {
            get => _areaComponent.Mask;
            set => _areaComponent.Mask = value;
        }

        public PointF? FindClosestPoint(PointF point, out float distance)
        {
            return _areaComponent.FindClosestPoint(point, out distance);
        }

        public bool IsInArea(PointF point) => _areaComponent.IsInArea(point);

        public bool IsInArea(PointF point, AGSBoundingBox projectionBox, float scaleX, float scaleY)
        {
            return _areaComponent.IsInArea(point, projectionBox, scaleX, scaleY);
        }
    }
}
