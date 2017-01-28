namespace AGS.API
{
    /// <summary>
    /// Contains information about UI controls which have focus (and thus block input from other controls).
    /// </summary>
    public interface IFocusedUI
    {
        /// <summary>
        /// Gets or sets the focused text box.
        /// If a text box is focused, then keyboard input should go to the text box and not to anywhere else.
        /// </summary>
        /// <value>The focused text box.</value>
        ITextBoxComponent FocusedTextBox { get; set; }

        /// <summary>
        /// Gets the focused window (useful for modal dialogs).
        /// If a window has focus then objects outside the window will not be clickable.
        /// To make your window focusable, add the IModalWindowComponent and then use GrabFocus/LoseFocus
        /// as needed.
        /// </summary>
        /// <value>The focused window.</value>
        IEntity FocusedWindow { get; }
    }
}
