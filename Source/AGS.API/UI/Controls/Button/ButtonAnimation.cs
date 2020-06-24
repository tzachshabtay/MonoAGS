using System.Collections.Generic;

namespace AGS.API
{
    /// <summary>
    /// Allows changing properties of the button to "animate" it as it moves between states (idle, hover, pushed).
    /// Each <see cref="ButtonAnimation"/> represents one of this states, and can have a different animation (or image),
    /// border, background color (tint) or text rendering configuration (font, shadows, etc- <see cref="ITextConfig"/>).
    /// </summary>
    [PropertyFolder]
    [ConcreteImplementation(DisplayName = "Button State Animation")]
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
        /// <param name="image">Image.</param>
        /// <param name="tint">Tint.</param>
        public ButtonAnimation(IImage image, Color? tint = null)
        {
            Image = image;
            Tint = tint;
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

        [MethodWizard]
        public ButtonAnimation(IAnimation animation, IBorderStyle border, ITextConfig textConfig, Color? tint)
        {
            Border = border;
            TextConfig = textConfig;
            Tint = tint;
            Animation = animation;
        }

        public ButtonAnimation(IAnimation animation, IImage image, IBorderStyle border, ITextConfig textConfig, Color? tint)
        {
            Border = border;
            TextConfig = textConfig;
            Tint = tint;
            Animation = animation;
            Image = image;
        }

        /// <summary>
        /// Gets or sets the image.
        /// </summary>
        public IImage Image { get; }

        /// <summary>
        /// Gets or sets the animation.
        /// </summary>
        /// <value>The animation.</value>
        public IAnimation Animation { get; }

        /// <summary>
        /// Gets or sets the border.
        /// </summary>
        /// <value>The border.</value>
        public IBorderStyle Border { get; }

        /// <summary>
        /// Gets or sets the text rendering configuration.
        /// </summary>
        /// <value>The text config.</value>
        public ITextConfig TextConfig { get; }

        /// <summary>
        /// Gets or sets the tint (background color).
        /// </summary>
        /// <value>The tint.</value>
        public Color? Tint { get; }

        public override string ToString()
        {
            if (Animation == null && Image == null && Border == null && TextConfig == null && Tint == null)
            {
                return "No Changes";
            }
            List<string> builder = new List<string>();
            if (Animation != null) builder.Add($"Animation");
            if (Image != null) builder.Add($"Image");
            if (Border != null) builder.Add($"Border");
            if (TextConfig != null) builder.Add($"Text");
            if (Tint != null) builder.Add($"Tint");

            string changes = string.Join(", ", builder);
            return $"Changing: {changes}";
        }
    }
}
