namespace AGS.API
{
    /// <summary>
    /// How to skip the text?
    /// </summary>
    public enum SkipText
	{
        /// <summary>
        /// Skip the text if enough time has passed or if the user clicked the left mouse button.
        /// </summary>
        /// <seealso cref="ISayConfig.TextDelay"/>
		ByTimeAndMouse,
        /// <summary>
        /// Skip the text if enough time has passed.
        /// </summary>
        /// <seealso cref="ISayConfig.TextDelay"/>
		ByTime,
        /// <summary>
        /// Skip the text if user clicked the left mouse button.
        /// </summary>
		ByMouse,
        /// <summary>
        /// The engine will never skip the text itself, but will count on the developer to implement custom skipping (by 
        /// subsribing to the <see cref="ISayComponent.OnBeforeSay"/> event and calling the "SkipText" action when appropriate). 
        /// </summary>
		External,
	}

    /// <summary>
    /// Configuration for how speech is rendered.
    /// </summary>
    public interface ISayConfig
	{
        /// <summary>
        /// Configuration for how the text is rendered (font, color, outlines, etc).
        /// </summary>
        /// <value>The text config.</value>
		ITextConfig TextConfig { get; set; }

        /// <summary>
        /// When <see cref="SkipText"/> is configured to be either <see cref="SkipText.ByTime"/> or <see cref="SkipText.ByTimeAndMouse"/> 
        /// then the engine estimates how long to keep the said text on the screen before moving on. This property determines
        /// how long (in milliseconds) should the engine wait for each character in the text (and 40 milliseconds overall are added on top of that to avoid
        /// too short sentences ending abruptly). The default for this value is 70 milliseconds.
        /// </summary>
        /// <example>
        /// If the character says "Hello, world!" and TextDelay = 70, then the amount of the time the engine waits will be:
        /// 40 + number of characters * 70 = 40 + 13 * 70 = 950 (milliseconds).
        /// </example>
        /// <value>The text delay.</value>
		int TextDelay { get; set; }

        /// <summary>
        /// Determines how to skip the said text- by time, mouse, both (the default) or a custom implementation.
        /// </summary>
        /// <value>The skip text.</value>
		SkipText SkipText { get; set; }

        /// <summary>
        /// Gets or sets the size of the label which will host the text: default is (250, 200).
        /// </summary>
        /// <value>The size of the label.</value>
		SizeF LabelSize { get; set; }

        /// <summary>
        /// Gets or sets an optional border which will surround the label hosting the text.
        /// </summary>
        /// <value>The border.</value>
		IBorderStyle Border { get; set; }

        /// <summary>
        /// Gets or sets the color of the text label's background (the default is transparent).
        /// </summary>
        /// <value>The color of the background.</value>
		Color BackgroundColor { get; set; }

        /// <summary>
        /// An optional offset for rendering the text from the planned location.
        /// </summary>
        /// <value>The text offset.</value>
        PointF TextOffset { get; set; }

        /// <summary>
        /// A configuration for rendering a portrait. If this is null, no portrait will be rendered.
        /// </summary>
        /// <value>The portrait config.</value>
        IPortraitConfig PortraitConfig { get; set; }
	}
}

