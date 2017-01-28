namespace AGS.API
{
    /// <summary>
    /// Adds the ability for a window to be modal (blocks input from other windows).
    /// For example, if you create a "Save Game" dialog, you wouldn't want to be able to open the inventory
    /// when the dialog is showing, so you'll make the dialog a modal window, then you can grab focus 
    /// when the window is shown, and lose focus when the window is closed.
    /// </summary>
    public interface IModalWindowComponent : IComponent
    {
        /// <summary>
        /// Gets a value indicating whether this <see cref="T:AGS.API.IModalWindowComponent"/> has focus.
        /// </summary>
        /// <value><c>true</c> if has focus; otherwise, <c>false</c>.</value>
        bool HasFocus { get; }

        /// <summary>
        /// Grabs the focus for the window. Use this when opening your window.
        /// If a window grabs focus when another window already has focus, the new window
        /// will now be focused, and the engine will remember that the old window was focused before,
        /// so once the new window loses focus the old window will regain focus. 
        /// </summary>
        void GrabFocus();

        /// <summary>
        /// Loses the focus for the window, bringing the focus back to the previous focused window (or none,
        /// if no windows are focused, which is the default).
        /// </summary>
        void LoseFocus();
    }
}
