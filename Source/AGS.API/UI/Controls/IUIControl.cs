namespace AGS.API
{
    /// <summary>
    /// The UI events component adds the ability to subscribe to multiple events which are needed by GUIs \
    /// (like mouse click, enter, leave).
    /// </summary>
	[RequiredComponent(typeof(IEnabledComponent))]
	[RequiredComponent(typeof(IVisibleComponent))]
	[RequiredComponent(typeof(IColliderComponent))]
	public interface IUIEvents : IComponent
	{
        /// <summary>
        /// An event which is triggered whenever the mouse enters the boundaries of the entity.
        /// </summary>
        /// <value>The event.</value>
		IEvent<MousePositionEventArgs> MouseEnter { get; }

        /// <summary>
        /// An event which is triggered whenever the mouse leaves the boundaries of the entity.
        /// </summary>
        /// <value>The event.</value>
		IEvent<MousePositionEventArgs> MouseLeave { get; }

        /// <summary>
        /// An event which is triggered whenever the mouse moves within the boundaries of the entity.
        /// </summary>
        /// <value>The event.</value>
		IEvent<MousePositionEventArgs> MouseMove { get; }

        /// <summary>
        /// An event which is triggered whenever the mouse is clicked within the boundaries of the entity.
        /// Note that a mouse click is mouse button pressed down, swiftly followed by button release.
        /// </summary>
        /// <value>The event.</value>
		IEvent<MouseButtonEventArgs> MouseClicked { get; }

        /// <summary>
        /// An event which is triggered whenever the mouse is double clicked within the boundaries of the entity.
        /// Note that a mouse double click is 2 swift mouse button clicks in a row (of the same button).
        /// </summary>
        /// <value>The event.</value>
        IEvent<MouseButtonEventArgs> MouseDoubleClicked { get; }

        /// <summary>
        /// An event which is triggered whenever a mouse button is pressed down within the boundaries of the entity.
        /// </summary>
        /// <value>The event.</value>
        IEvent<MouseButtonEventArgs> MouseDown { get; }

        /// <summary>
        /// An event which is triggered whenever a mouse button is released, if the button was pressed down within the boundaries of the entity.
        /// </summary>
        /// <value>The event.</value>
		IEvent<MouseButtonEventArgs> MouseUp { get; }

        /// <summary>
        /// An event which is triggered whenever the entity loses focus (a mouse button was clicked outside 
        /// the boundaries of the entity).
        /// </summary>
        /// <value>The lost focus.</value>
        IEvent<MouseButtonEventArgs> LostFocus { get; }

        /// <summary>
        /// Gets a value indicating whether the mouse cursor is within the entity's boundaries.
        /// </summary>
        /// <value><c>true</c> if is mouse in; otherwise, <c>false</c>.</value>
        bool IsMouseIn { get; }
	}

    /// <summary>
    /// A UI control is an object which has events like mouse click, to help the user interact with it.
    /// A UI control also has a skin component, for customing its look.
    /// All other GUIs (buttons, checkboxes, textboxes, etc) are also a UI control.
    /// </summary>
	public interface IUIControl : IUIEvents, ISkinComponent, IObject
	{
	}
}

