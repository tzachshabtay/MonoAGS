using AGS.API;

namespace AGS.Engine
{
    public class AGSRotate : IRotate
    {
        private readonly AGSEventArgs _args = new AGSEventArgs();
        private float _angle;

        public AGSRotate()
        {
            OnAngleChanged = new AGSEvent<AGSEventArgs>();
        }

        public float Angle { get { return _angle; } set { _angle = value; fireAngleChanged(); } }

        public IEvent<AGSEventArgs> OnAngleChanged { get; private set; }

        private void fireAngleChanged()
        {
            OnAngleChanged.FireEvent(this, _args);
        }
    }
}
