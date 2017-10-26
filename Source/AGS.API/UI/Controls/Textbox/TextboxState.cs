namespace AGS.API
{
    /// <summary>
    /// Represents the state in which the textbox is currently in.
    /// </summary>
    /// <seealso cref="TextBoxKeyPressingEventArgs.IntendedState"/>
    public class TextboxState
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="T:AGS.API.TextboxState"/> class.
        /// </summary>
        /// <param name="text">Text.</param>
        /// <param name="caretPosition">Caret position.</param>
        public TextboxState(string text, int caretPosition)
        {
            Text = text;
            CaretPosition = caretPosition;
        }

        /// <summary>
        /// Gets or sets the text.
        /// </summary>
        /// <value>The text.</value>
        public string Text { get; set; }

        /// <summary>
        /// Gets or sets the caret position.
        /// </summary>
        /// <value>The caret position.</value>
        public int CaretPosition { get; set; }
    }
}
