namespace AGS.API
{
    /// <summary>
    /// Allows changing properties of the button to "animate" it as it moves between states (idle, hover, pushed).
    /// Each <see cref="ButtonAnimation"/> represents one of this states, and can have a different animation,
    /// border, background color (tint) or text rendering configuration (font, shadows, etc- <see cref="ITextConfig"/>).
    /// </summary>
    public class ButtonAnimation
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="T:AGS.API.ButtonAnimation"/> class.
        /// </summary>
        /// <param name="animation">Animation.</param>
        public ButtonAnimation(IAnimation animation)
        {
            Animation = animation;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:AGS.API.ButtonAnimation"/> class.
        /// </summary>
        /// <param name="border">Border.</param>
        /// <param name="textConfig">Text config.</param>
        /// <param name="tint">Tint.</param>
        public ButtonAnimation(IBorderStyle border, ITextConfig textConfig, Color? tint)
        {
            Border = border;
            TextConfig = textConfig;
            Tint = tint;
        }

        /// <summary>
        /// Gets or sets the animation.
        /// </summary>
        /// <value>The animation.</value>
        public IAnimation Animation { get; set; }

        /// <summary>
        /// Gets or sets the border.
        /// </summary>
        /// <value>The border.</value>
        public IBorderStyle Border { get; set; }

        /// <summary>
        /// Gets or sets the text rendering configuration.
        /// </summary>
        /// <value>The text config.</value>
        public ITextConfig TextConfig { get; set; }

        /// <summary>
        /// Gets or sets the tint (background color).
        /// </summary>
        /// <value>The tint.</value>
        public Color? Tint { get; set; }
    }
}
