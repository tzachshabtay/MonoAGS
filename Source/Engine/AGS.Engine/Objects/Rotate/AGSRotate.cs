using AGS.API;

namespace AGS.Engine
{
    public class AGSRotate : IRotate
    {
        private float _angle;

        public AGSRotate()
        {
            OnAngleChanged = new AGSEvent<object>();
        }

        public float Angle { get { return _angle; } set { _angle = value; fireAngleChanged(); } }

        public IEvent<object> OnAngleChanged { get; private set; }

        private void fireAngleChanged()
        {
            OnAngleChanged.FireEvent(null);
        }
    }
}
