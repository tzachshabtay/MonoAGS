using AGS.API;

namespace AGS.Engine
{
	public class AGSButtonComponent : AGSComponent, IButtonComponent
	{
		private IUIEvents _events;
        private ISpriteRenderComponent _spriteRender;
		private IAnimationComponent _animation;
        private ITextComponent _text;
        private IImageComponent _image;

		public override void Init(IEntity entity)
		{
			base.Init(entity);
            entity.Bind<ISpriteRenderComponent>(c => _spriteRender = c, _ => _spriteRender = null);
            entity.Bind<IAnimationComponent>(c => _animation = c, _ => _animation = null);
            entity.Bind<ITextComponent>(c => _text = c, _ => _text = null);
            entity.Bind<IImageComponent>(c => _image = c, _ => _image = null);
            entity.Bind<IUIEvents>(c =>
            {
                _events = c;
                c.MouseEnter.Subscribe(onMouseEnter);
                c.MouseLeave.Subscribe(onMouseLeave);
                c.MouseDown.Subscribe(onMouseDown);
                c.MouseUp.Subscribe(onMouseUp);
            }, c =>
            {
                _events = null;
                c.MouseEnter.Unsubscribe(onMouseEnter);
                c.MouseLeave.Unsubscribe(onMouseLeave);
                c.MouseDown.Unsubscribe(onMouseDown);
                c.MouseUp.Unsubscribe(onMouseUp);
            });
		}

        public ButtonAnimation IdleAnimation { get; set; }

        public ButtonAnimation HoverAnimation { get; set; }

        public ButtonAnimation PushedAnimation { get; set; }

		private void onMouseEnter(MousePositionEventArgs e)
		{
            startAnimation(HoverAnimation ?? IdleAnimation ?? PushedAnimation);
		}

		private void onMouseLeave(MousePositionEventArgs e)
		{
            startAnimation(IdleAnimation ?? PushedAnimation ?? HoverAnimation);
		}

		private void onMouseDown(MouseButtonEventArgs e)
		{
            startAnimation(PushedAnimation ?? HoverAnimation ?? IdleAnimation);
		}

		private void onMouseUp(MouseButtonEventArgs e)
		{
            startAnimation(_events.IsMouseIn ?
                           HoverAnimation ?? IdleAnimation ?? PushedAnimation :
                           IdleAnimation ?? HoverAnimation ?? PushedAnimation);
		}

        private void startAnimation(ButtonAnimation button)
        {
            button.StartAnimation(_animation, _text, _image, _spriteRender);
        }
	}
}

