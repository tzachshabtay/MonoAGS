using System;

namespace AGS.API
{
    public interface IUIEvents : IDisposable
	{
		IEvent<MousePositionEventArgs> MouseEnter { get; }
		IEvent<MousePositionEventArgs> MouseLeave { get; }
		IEvent<MousePositionEventArgs> MouseMove { get; }
		IEvent<MouseButtonEventArgs> MouseClicked { get; }
		IEvent<MouseButtonEventArgs> MouseDown { get; }
		IEvent<MouseButtonEventArgs> MouseUp { get; }

		bool IsMouseIn { get; }
	}

	public interface IUIControl<TControl> : IObject where TControl : IUIControl<TControl>
	{
		IUIEvents Events { get; }

		void ApplySkin(TControl skin);
	}
}

