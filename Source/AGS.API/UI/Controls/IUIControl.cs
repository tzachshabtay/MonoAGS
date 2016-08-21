namespace AGS.API
{
	[RequiredComponent(typeof(IEnabledComponent))]
	[RequiredComponent(typeof(IVisibleComponent))]
	[RequiredComponent(typeof(ICollider))]
	public interface IUIEvents : IComponent
	{
		IEvent<MousePositionEventArgs> MouseEnter { get; }
		IEvent<MousePositionEventArgs> MouseLeave { get; }
		IEvent<MousePositionEventArgs> MouseMove { get; }
		IEvent<MouseButtonEventArgs> MouseClicked { get; }
        IEvent<MouseButtonEventArgs> MouseDoubleClicked { get; }
        IEvent<MouseButtonEventArgs> MouseDown { get; }
		IEvent<MouseButtonEventArgs> MouseUp { get; }
        IEvent<MouseButtonEventArgs> LostFocus { get; }

        bool IsMouseIn { get; }
	}

	public interface IUIControl : IUIEvents, ISkinComponent, IObject
	{
	}
}

