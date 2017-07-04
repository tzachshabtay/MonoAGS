using AGS.API;

namespace AGS.Engine
{
    public partial class AGSArea : AGSEntity, IArea
    {
        private IAreaComponent _areaComponent;

        public AGSArea(string id, Resolver resolver) : base(id, resolver)
        {
            _areaComponent = AddComponent<IAreaComponent>();

            beforeInitComponents(resolver);
            InitComponents();
            afterInitComponents(resolver);
        }

        public string Name { get { return ID; } }
        public bool AllowMultiple { get { return false; } }
        public void Init(IEntity entity) { }

        public override string ToString()
        {
            return string.Format("{0} ({1})", ID ?? "", GetType().Name);
        }

        partial void beforeInitComponents(Resolver resolver);
        partial void afterInitComponents(Resolver resolver);

        public bool Enabled
        {
            get { return _areaComponent.Enabled; }
            set { _areaComponent.Enabled = value; }
        }

        public IMask Mask
        {
            get { return _areaComponent.Mask; }
            set { _areaComponent.Mask = value; }
        }

        public PointF? FindClosestPoint(PointF point, out float distance)
        {
            return _areaComponent.FindClosestPoint(point, out distance);
        }

        public bool IsInArea(PointF point)
        {
            return _areaComponent.IsInArea(point);
        }

        public bool IsInArea(PointF point, AGSSquare projectionBox, float scaleX, float scaleY)
        {
            return _areaComponent.IsInArea(point, projectionBox, scaleX, scaleY);
        }
    }
}
