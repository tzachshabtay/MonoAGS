using System;

namespace AGS.API
{
	[RequiredComponent(typeof(IEnabledComponent))]
	[RequiredComponent(typeof(IVisibleComponent))]
	[RequiredComponent(typeof(IDrawableInfo))]
	[RequiredComponent(typeof(ICollider))]
	public interface IUIEvents : IComponent
	{
		IEvent<MousePositionEventArgs> MouseEnter { get; }
		IEvent<MousePositionEventArgs> MouseLeave { get; }
		IEvent<MousePositionEventArgs> MouseMove { get; }
		IEvent<MouseButtonEventArgs> MouseClicked { get; }
		IEvent<MouseButtonEventArgs> MouseDown { get; }
		IEvent<MouseButtonEventArgs> MouseUp { get; }

		bool IsMouseIn { get; }
	}

	public interface IUIControl<TControl> : IUIEvents, IObject where TControl : IUIControl<TControl>
	{
		void ApplySkin(TControl skin);
	}
}

