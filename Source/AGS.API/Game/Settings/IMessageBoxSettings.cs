namespace AGS.API
{
    /// <summary>
    /// Settings for creating message boxes. Those are created from the "AGSMessageBox" class.
    /// </summary>
    public interface IMessageBoxSettings
    {
        /// <summary>
        /// Gets or sets the render layer.
        /// </summary>
        /// <value>The render layer.</value>
        IRenderLayer RenderLayer { get; set; }

        /// <summary>
        /// Gets or sets the display configuration.
        /// </summary>
        /// <value>The display config.</value>
        ISayConfig DisplayConfig { get; set; }

        /// <summary>
        /// Gets or sets the text configuration for the buttons in the message box (if there are any buttons).
        /// </summary>
        /// <value>The button text.</value>
        ITextConfig ButtonText { get; set; }

        /// <summary>
        /// Gets or sets the buttons padding on the x axis (if there are any buttons).
        /// </summary>
        /// <value>The button X padding.</value>
        float ButtonXPadding { get; set; }

        /// <summary>
        /// Gets or sets the buttons padding on the y axis (if there are any buttons).
        /// </summary>
        /// <value>The button Y padding.</value>
        float ButtonYPadding { get; set; }

        /// <summary>
        /// Gets or sets the buttons width (if there are any buttons).
        /// </summary>
        /// <value>The button width.</value>
        float ButtonWidth { get; set; }

        /// <summary>
        /// Gets or sets the buttons height (if there are any buttons).
        /// </summary>
        /// <value>The button height.</value>
        float ButtonHeight { get; set; }
    }
}