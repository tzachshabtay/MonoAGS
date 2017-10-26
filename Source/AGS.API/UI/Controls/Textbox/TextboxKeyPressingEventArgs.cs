namespace AGS.API
{
    /// <summary>
    /// Event arguments for when a key is pressed on the textbox.
    /// </summary>
    public class TextBoxKeyPressingEventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="T:AGS.API.TextBoxKeyPressingEventArgs"/> class.
        /// </summary>
        /// <param name="pressedKey">Pressed key.</param>
        /// <param name="intendedState">Intended state.</param>
        public TextBoxKeyPressingEventArgs(Key pressedKey, TextboxState intendedState)
        {
            PressedKey = pressedKey;
            IntendedState = intendedState;
        }

        /// <summary>
        /// Gets the pressed key.
        /// </summary>
        /// <value>The pressed key.</value>
        public Key PressedKey { get; private set; }

        /// <summary>
        /// A subscriber of the event can canel processing the key input by setting this property to true.
        /// </summary>
        /// <value><c>true</c> if should cancel; otherwise, <c>false</c>.</value>
        public bool ShouldCancel { get; set; }

        /// <summary>
        /// Gets the state the textbox is intending to move to once the current operation is completed.
        /// You can edit this state when receiving an <see cref="ITextBoxComponent.OnPressingKey"/> event
        /// to override the result (i.e change the text or caret position).
        /// </summary>
        /// <value>The state of the intended.</value>
        public TextboxState IntendedState { get; private set; }
    }
}
