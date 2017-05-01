namespace AGS.API
{
    /// <summary>
    /// A text component allows displaying text on the screen.
    /// </summary>
    [RequiredComponent(typeof(IImageComponent))]
    public interface ITextComponent : IComponent
    {
        /// <summary>
        /// Gets or sets the text configuration (font, color, outline, etc).
        /// </summary>
        /// <value>The text config.</value>
        ITextConfig TextConfig { get; set; }

        /// <summary>
        /// Gets or sets the text.
        /// </summary>
        /// <value>The text.</value>
        string Text { get; set; }

        /// <summary>
        /// Gets or sets the size of the label that hosts the text (that label can have a color and a border,
        /// and is also used for text alignment calculations).
        /// </summary>
        /// <value>The size of the label render.</value>
        SizeF LabelRenderSize { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to show/hide the text..
        /// </summary>
        /// <value><c>true</c> if text visible; otherwise, <c>false</c>.</value>
        bool TextVisible { get; set; }

        /// <summary>
        /// Gets the height of the text.
        /// </summary>
        /// <value>The height of the text.</value>
        float TextHeight { get; }

        /// <summary>
        /// Gets the width of the text.
        /// </summary>
        /// <value>The width of the text.</value>
        float TextWidth { get; }
    }
}
