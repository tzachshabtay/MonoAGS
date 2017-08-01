using AGS.API;

namespace AGS.Engine
{
    public class AGSJumpOffsetComponent : AGSComponent, IJumpOffsetComponent
    {
        private PointF _jumpOffset;

        public AGSJumpOffsetComponent()
        {
            OnJumpOffsetChanged = new AGSEvent<object>();
        }

        public PointF JumpOffset 
        {
            get { return _jumpOffset; }
            set 
            {
                if (_jumpOffset.Equals(value)) return;
                _jumpOffset = value;
                OnJumpOffsetChanged.Invoke(null);
            }
        }

        public IEvent<object> OnJumpOffsetChanged { get; private set; }
     }
}
