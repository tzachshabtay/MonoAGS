namespace AGS.API
{
    /// <summary>
    /// Responsible for rendering labels (which show text).
    /// </summary>
    public interface ILabelRenderer : IImageRenderer
	{
        /// <summary>
        /// Gets or sets the text to render.
        /// </summary>
        /// <value>The text.</value>
		string Text { get; set; }

        /// <summary>
        /// Gets or sets the text rendering configuration (font, color, outline, etc).
        /// </summary>
        /// <value>The config.</value>
		ITextConfig Config { get; set; }

        /// <summary>
        /// Gets or sets the base size of the label (which hosts the text), before doing all the configuration
        /// related processing.
        /// </summary>
        /// <value>The size of the base.</value>
	    SizeF BaseSize { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the text should be rendered.
        /// </summary>
        /// <value><c>true</c> if text visible; otherwise, <c>false</c>.</value>
        bool TextVisible { get; set; }

        /// <summary>
        /// Gets or sets the caret position (for text input).
        /// The position is by number of characters (0 is before the first character).
        /// </summary>
        /// <value>The caret position.</value>
        int CaretPosition { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the caret (for text input) should be rendered.
        /// </summary>
        /// <value><c>true</c> if render caret; otherwise, <c>false</c>.</value>
        bool RenderCaret { get; set; }

        /// <summary>
        /// Gets the actual width of the label.
        /// </summary>
        /// <value>The width.</value>
		float Width { get; }

        /// <summary>
        /// Gets the actual height of the label.
        /// </summary>
        /// <value>The height.</value>
		float Height { get; }

        /// <summary>
        /// Gets the width of the text.
        /// </summary>
        /// <value>The width of the text.</value>
		float TextWidth { get; }

        /// <summary>
        /// Gets the height of the text.
        /// </summary>
        /// <value>The height of the text.</value>
		float TextHeight { get; }

        /// <summary>
        /// Event which fires whenever the label size changes.
        /// </summary>
        /// <value>The on label size changed.</value>
        IEvent<object> OnLabelSizeChanged { get; }
	}
}
