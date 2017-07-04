using AGS.API;

namespace AGS.Engine.UI.Controls
{
    public class AGSCheckboxComponent : AGSComponent, ICheckboxComponent
    {
        private bool _checked;
        private IUIEvents _events;
        private IAnimationContainer _animation;
        private ITextComponent _text;
        private IImageComponent _image;

        public AGSCheckboxComponent()
        {
            OnCheckChanged = new AGSEvent<CheckBoxEventArgs>();
        }

        public override void Init(IEntity entity)
        {
            base.Init(entity);
            _events = entity.GetComponent<IUIEvents>();
            _animation = entity.GetComponent<IAnimationContainer>();
            _text = entity.GetComponent<ITextComponent>();
            _image = entity.GetComponent<IImageComponent>();

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

        public ButtonAnimation CheckedAnimation { get; set; }

        public ButtonAnimation HoverCheckedAnimation { get; set; }

        public ButtonAnimation HoverNotCheckedAnimation { get; set; }

        public ButtonAnimation NotCheckedAnimation { get; set; }

        public IEvent<CheckBoxEventArgs> OnCheckChanged { get; private set; }

        public override void Dispose()
        {
            _events.MouseEnter.Unsubscribe(onMouseEnter);
            _events.MouseLeave.Unsubscribe(onMouseLeave);          
            _events.MouseUp.Unsubscribe(onMouseUp);
        }

        private void onMouseEnter(MousePositionEventArgs e)
        {
            startAnimation(Checked ? HoverCheckedAnimation ?? CheckedAnimation : HoverNotCheckedAnimation ?? NotCheckedAnimation);
        }

        private void onMouseLeave(MousePositionEventArgs e)
        {
            startAnimation(Checked ? CheckedAnimation : NotCheckedAnimation);
        }

        private void onMouseUp(MouseButtonEventArgs e)
        {
            if (_events.IsMouseIn)
            {
                _checked = !_checked;
            }
            onCheckChange();
        }

        private void onCheckChange()
        {
            startAnimation(_events.IsMouseIn ? (Checked ? HoverCheckedAnimation ?? CheckedAnimation : 
                                                          HoverNotCheckedAnimation ?? NotCheckedAnimation) :
                (Checked ? CheckedAnimation : NotCheckedAnimation));
            OnCheckChanged.Invoke(new CheckBoxEventArgs(Checked));
        }

        private void startAnimation(ButtonAnimation button)
        {
	        button.StartAnimation(_animation, _text, _image);
        }
    }
}
