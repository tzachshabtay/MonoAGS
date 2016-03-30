using System;
using AGS.API;

namespace AGS.Engine
{
	public class AGSButtonComponent : AGSComponent, IButtonComponent
	{
		private IUIEvents _events;
		private IAnimationContainer _animation;

		public AGSButtonComponent()
		{
		}

		public override void Init(IEntity entity)
		{
			base.Init(entity);
			_events = entity.GetComponent<IUIEvents>();
			_animation = entity.GetComponent<IAnimationContainer>();

			_events.MouseEnter.Subscribe(onMouseEnter);
			_events.MouseLeave.Subscribe(onMouseLeave);
			_events.MouseDown.Subscribe(onMouseDown);
			_events.MouseUp.Subscribe(onMouseUp);
		}

		public IAnimation IdleAnimation { get; set; }

		public IAnimation HoverAnimation { get; set; }

		public IAnimation PushedAnimation { get; set; }

		public override void Dispose()
		{
			_events.MouseEnter.Unsubscribe(onMouseEnter);
			_events.MouseLeave.Unsubscribe(onMouseLeave);
			_events.MouseDown.Unsubscribe(onMouseDown);
			_events.MouseUp.Unsubscribe(onMouseUp);
		}

		private void onMouseEnter(object sender, MousePositionEventArgs e)
		{
			_animation.StartAnimation(HoverAnimation);
		}

		private void onMouseLeave(object sender, MousePositionEventArgs e)
		{
			_animation.StartAnimation(IdleAnimation);
		}

		private void onMouseDown(object sender, MouseButtonEventArgs e)
		{
			_animation.StartAnimation(PushedAnimation);
		}

		private void onMouseUp(object sender, MouseButtonEventArgs e)
		{
			_animation.StartAnimation(_events.IsMouseIn ? HoverAnimation : IdleAnimation);
		}
	}
}

