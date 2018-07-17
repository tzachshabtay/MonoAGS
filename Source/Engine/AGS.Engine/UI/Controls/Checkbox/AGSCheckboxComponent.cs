using System;
using System.ComponentModel;
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
        private IBorderComponent _border;
        private IRadioGroup _radioGroup;

        public AGSCheckboxComponent()
        {
            OnCheckChanged = new AGSEvent<CheckBoxEventArgs>();
        }

        public override void Init()
        {
            base.Init();
            Entity.Bind<IAnimationComponent>(c => _animation = c, _ => _animation = null);
            Entity.Bind<ITextComponent>(c => _text = c, _ => _text = null);
            Entity.Bind<IImageComponent>(c => _image = c, _ => _image = null);
            Entity.Bind<IBorderComponent>(c => _border = c, _ => _border = null);
            Entity.Bind<IUIEvents>(c =>
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
                onCheckChange(false);
            }
        }

        public ButtonAnimation CheckedAnimation { get; set; }

        public ButtonAnimation HoverCheckedAnimation { get; set; }

        public ButtonAnimation HoverNotCheckedAnimation { get; set; }

        public ButtonAnimation NotCheckedAnimation { get; set; }

        public ILabel TextLabel { get; set; }

        public IBlockingEvent<CheckBoxEventArgs> OnCheckChanged { get; private set; }

        public IRadioGroup RadioGroup
        {
            get => _radioGroup;
            set 
            {
                var existing = _radioGroup;
                if (existing != null)
                {
                    existing.PropertyChanged -= onRadioPropertyChanged;
                }
                if (value != null)
                {
                    value.PropertyChanged += onRadioPropertyChanged;
                }
                _radioGroup = value;
            }
        }

        private void onRadioPropertyChanged(object sender, PropertyChangedEventArgs args)
        {
            if (args.PropertyName != nameof(IRadioGroup.SelectedButton)) return;
            var radioGroup = _radioGroup;
            if (radioGroup == null) return;
            bool isMe = radioGroup.SelectedButton == this || radioGroup.SelectedButton == Entity;
            if (isMe && !_checked) Checked = true;
            else if (!isMe && _checked) Checked = false;
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
            var events = _events;
            if (events?.IsMouseIn ?? false)
            {
                _checked = !_checked;
                onCheckChange(true);
            }
        }

        private void onCheckChange(bool userInitiated)
        {
            var radio = _radioGroup;
            if (radio != null && _checked)
            {
                radio.SelectedButton = this;
            }
            var events = _events;
            startAnimation(events != null && events.IsMouseIn ? (Checked ? HoverCheckedAnimation ?? CheckedAnimation : 
                                                          HoverNotCheckedAnimation ?? NotCheckedAnimation) :
                (Checked ? CheckedAnimation : NotCheckedAnimation));
            OnCheckChanged.Invoke(new CheckBoxEventArgs(Checked, userInitiated));
        }

        private void startAnimation(ButtonAnimation button)
        {
	        button.StartAnimation(_animation, _text, _image, _border);
        }
    }
}
