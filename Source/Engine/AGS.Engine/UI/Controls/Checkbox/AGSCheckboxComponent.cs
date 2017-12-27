using AGS.API;

namespace AGS.Engine.UI.Controls
{
    public class AGSCheckboxComponent : AGSComponent, ICheckboxComponent
    {
        private bool _checked;
        private IUIEvents _events;
        private IAnimationComponent _animation;
        private ITextComponent _text;
        private IImageComponent _image;

        public AGSCheckboxComponent()
        {
            OnCheckChanged = new AGSEvent<CheckBoxEventArgs>();
        }

        public override void Init(IEntity entity)
        {
            base.Init(entity);
			entity.Bind<IAnimationComponent>(c => _animation = c, _ => _animation = null);
			entity.Bind<ITextComponent>(c => _text = c, _ => _text = null);
			entity.Bind<IImageComponent>(c => _image = c, _ => _image = null);
			entity.Bind<IUIEvents>(c =>
			{
				_events = c;
				c.MouseEnter.Subscribe(onMouseEnter);
				c.MouseLeave.Subscribe(onMouseLeave);
				c.MouseUp.Subscribe(onMouseUp);
			}, c =>
			{
				_events = null;
				c.MouseEnter.Unsubscribe(onMouseEnter);
				c.MouseLeave.Unsubscribe(onMouseLeave);
				c.MouseUp.Unsubscribe(onMouseUp);
			});
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

        public ILabel TextLabel { get; set; }

        public IBlockingEvent<CheckBoxEventArgs> OnCheckChanged { get; private set; }

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
            var events = _events;
            if (events?.IsMouseIn ?? false)
            {
                _checked = !_checked;
                onCheckChange();
            }
        }

        private void onCheckChange()
        {
            var events = _events;
            startAnimation(events != null && events.IsMouseIn ? (Checked ? HoverCheckedAnimation ?? CheckedAnimation : 
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
