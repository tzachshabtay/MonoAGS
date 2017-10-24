using AGS.API;

namespace AGS.Engine
{
    public class AGSRotateComponent : AGSComponent, IRotateComponent
    {
        private readonly IRotate _rotate;

        public AGSRotateComponent(IRotate rotate)
        {
            _rotate = rotate;
        }

        [Property(Category = "Transform")]
        public float Angle {  get { return _rotate.Angle; } set { _rotate.Angle = value; } }

        public IEvent OnAngleChanged { get { return _rotate.OnAngleChanged; } }
    }
}
