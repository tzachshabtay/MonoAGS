using AGS.API;

namespace AGS.Engine
{
    public class AGSJumpOffsetComponent : AGSComponent, IJumpOffsetComponent
    {
        private PointF _jumpOffset;

        public AGSJumpOffsetComponent()
        {
            OnJumpOffsetChanged = new AGSEvent();
        }

        public PointF JumpOffset 
        {
            get { return _jumpOffset; }
            set 
            {
                if (_jumpOffset.Equals(value)) return;
                _jumpOffset = value;
                OnJumpOffsetChanged.Invoke();
            }
        }

        public IEvent OnJumpOffsetChanged { get; private set; }
     }
}
