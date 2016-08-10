using AGS.API;

namespace AGS.Engine.UI.Controls
{
    public class AGSCheckboxComponent : AGSComponent, ICheckboxComponent
    {
        private bool _checked;
        private IUIEvents _events;
        private IAnimationContainer _animation;

        public AGSCheckboxComponent()
        {
            OnCheckChanged = new AGSEvent<CheckBoxEventArgs>();
        }

        public override void Init(IEntity entity)
        {
            base.Init(entity);
            _events = entity.GetComponent<IUIEvents>();
            _animation = entity.GetComponent<IAnimationContainer>();

            _events.MouseEnter.Subscribe(onMouseEnter);
            _events.MouseLeave.Subscribe(onMouseLeave);
            _events.MouseUp.Subscribe(onMouseUp);
        }

        public bool Checked
        {
            get { return _checked; }
            set
            {
                _checked = value;
                onCheckChange();
            }
        }

        public IAnimation CheckedAnimation { get; set; }

        public IAnimation HoverCheckedAnimation { get; set; }

        public IAnimation HoverNotCheckedAnimation { get; set; }

        public IAnimation NotCheckedAnimation { get; set; }

        public IEvent<CheckBoxEventArgs> OnCheckChanged { get; private set; }

        public override void Dispose()
        {
            _events.MouseEnter.Unsubscribe(onMouseEnter);
            _events.MouseLeave.Unsubscribe(onMouseLeave);          
            _events.MouseUp.Unsubscribe(onMouseUp);
        }

        private void onMouseEnter(object sender, MousePositionEventArgs e)
        {
            _animation.StartAnimation(Checked ? HoverCheckedAnimation : HoverNotCheckedAnimation);
        }

        private void onMouseLeave(object sender, MousePositionEventArgs e)
        {
            _animation.StartAnimation(Checked ? CheckedAnimation : NotCheckedAnimation);
        }

        private void onMouseUp(object sender, MouseButtonEventArgs e)
        {
            if (_events.IsMouseIn)
            {
                _checked = !_checked;
            }
            onCheckChange();
        }

        private void onCheckChange()
        {
            _animation.StartAnimation(_events.IsMouseIn ? (Checked ? HoverCheckedAnimation : HoverNotCheckedAnimation) :
                (Checked ? CheckedAnimation : NotCheckedAnimation));
            OnCheckChanged.Invoke(this, new CheckBoxEventArgs(Checked));
        }
    }
}
