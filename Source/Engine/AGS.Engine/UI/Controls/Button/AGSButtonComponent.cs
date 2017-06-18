using AGS.API;

namespace AGS.Engine
{
	public class AGSButtonComponent : AGSComponent, IButtonComponent
	{
		private IUIEvents _events;
		private IAnimationContainer _animation;
        private ITextComponent _text;
        private IImageComponent _image;

		public override void Init(IEntity entity)
		{
			base.Init(entity);
			_events = entity.GetComponent<IUIEvents>();
			_animation = entity.GetComponent<IAnimationContainer>();
            _text = entity.GetComponent<ITextComponent>();
            _image = entity.GetComponent<IImageComponent>();

			_events.MouseEnter.Subscribe(onMouseEnter);
			_events.MouseLeave.Subscribe(onMouseLeave);
			_events.MouseDown.Subscribe(onMouseDown);
			_events.MouseUp.Subscribe(onMouseUp);
		}

        public ButtonAnimation IdleAnimation { get; set; }

        public ButtonAnimation HoverAnimation { get; set; }

        public ButtonAnimation PushedAnimation { get; set; }

		public override void Dispose()
		{
			_events.MouseEnter.Unsubscribe(onMouseEnter);
			_events.MouseLeave.Unsubscribe(onMouseLeave);
			_events.MouseDown.Unsubscribe(onMouseDown);
			_events.MouseUp.Unsubscribe(onMouseUp);
		}

		private void onMouseEnter(object sender, MousePositionEventArgs e)
		{
            startAnimation(HoverAnimation ?? IdleAnimation ?? PushedAnimation);
		}

		private void onMouseLeave(object sender, MousePositionEventArgs e)
		{
            startAnimation(IdleAnimation ?? PushedAnimation ?? HoverAnimation);
		}

		private void onMouseDown(object sender, MouseButtonEventArgs e)
		{
            startAnimation(PushedAnimation ?? HoverAnimation ?? IdleAnimation);
		}

		private void onMouseUp(object sender, MouseButtonEventArgs e)
		{
            startAnimation(_events.IsMouseIn ?
                           HoverAnimation ?? IdleAnimation ?? PushedAnimation :
                           IdleAnimation ?? HoverAnimation ?? PushedAnimation);
		}

        private void startAnimation(ButtonAnimation button)
        {
            button.StartAnimation(_animation, _text, _image);
        }
	}
}

