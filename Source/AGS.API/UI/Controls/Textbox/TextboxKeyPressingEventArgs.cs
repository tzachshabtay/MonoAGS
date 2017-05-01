namespace AGS.API
{
    /// <summary>
    /// Event arguments for when a key is pressed on the textbox.
    /// </summary>
    public class TextBoxKeyPressingEventArgs : AGSEventArgs
    {
        public TextBoxKeyPressingEventArgs(Key pressedKey)
        {
            PressedKey = pressedKey;
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
    }
}
