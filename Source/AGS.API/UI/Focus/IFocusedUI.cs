namespace AGS.API
{
    /// <summary>
    /// Contains information about UI controls which have focus (and thus block input from other controls).
    /// </summary>
    public interface IFocusedUI
    {
        /// <summary>
        /// Gets or sets the control which receives keyboard events.
        /// If there's a control with keyboard focus, then keyboard input should go to that control and not to anywhere else.
        /// </summary>
        /// <value>The focused text box.</value>
        IEntity HasKeyboardFocus { get; set; }

        /// <summary>
        /// Gets the focused window (useful for modal dialogs).
        /// If a window has focus then objects outside the window will not be clickable.
        /// To make your window focusable, add the IModalWindowComponent and then use GrabFocus/LoseFocus
        /// as needed.
        /// </summary>
        /// <value>The focused window.</value>
        IEntity FocusedWindow { get; }

        /// <summary>
        /// A list of entities that cannot lose focus. Those will still grab input even if there's a grabbing focus dialog
        /// showing. This is useful for debugging tools, for example.
        /// </summary>
        /// <value>The cannot lose focus.</value>
        IConcurrentHashSet<string> CannotLoseFocus { get; }
    }
}
