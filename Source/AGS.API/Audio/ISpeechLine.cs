namespace AGS.API
{
    /// <summary>
    /// Represents a line of speech (text and sound).
    /// </summary>
    public interface ISpeechLine
    {
        /// <summary>
        /// Gets the text to be rendered on screen for this line.
        /// </summary>
        /// <value>The text.</value>
        string Text { get; }

        /// <summary>
        /// Gets the audio clip to be played (or null if there's no speech file for this line).
        /// </summary>
        /// <value>The audio clip.</value>
        IAudioClip AudioClip { get; }
    }
}

