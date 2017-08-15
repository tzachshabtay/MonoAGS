using AGS.API;

namespace AGS.Engine
{
    public class AGSRotate : IRotate
    {
        private float _angle;

        public AGSRotate()
        {
            OnAngleChanged = new AGSEvent();
        }

        public float Angle { get { return _angle; } set { _angle = value; fireAngleChanged(); } }

        public IEvent OnAngleChanged { get; private set; }

        private void fireAngleChanged()
        {
            OnAngleChanged.Invoke();
        }
    }
}
