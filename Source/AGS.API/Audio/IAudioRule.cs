namespace AGS.API
{
    /// <summary>
    /// You can implement an audio rule and add it to <see cref="IAudioSystem.AudioRules"/>, to modify properties of playing sounds.
    /// It gets callbacks from the engines whenever a sound has played and completed, and can add a <see cref="ISoundModifier"/> to
    /// the sound (<see cref="ISound.SoundModifiers"/>) to modify its properties.
    /// 
    /// Note: the audio rule is a conviency interface for interacting all the sounds in the system. You don't have to use it, though, there's
    /// nothing stopping you from adding sound modifiers to sounds outside this system.
    /// </summary>
    public interface IAudioRule
    {
        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="T:AGS.API.IAudioRule"/> is enabled.
        /// </summary>
        /// <value><c>true</c> if enabled; otherwise, <c>false</c>.</value>
        bool Enabled { get; set; }

        /// <summary>
        /// Called by the engine to notify the rule that a new sound has started.
        /// </summary>
        /// <param name="sound">Sound.</param>
        void OnSoundStarted(ISound sound);

        /// <summary>
        /// Called by the engine to notify the rule that an existing sound has completed playing.
        /// </summary>
        /// <param name="sound">Sound.</param>
        void OnSoundCompleted(ISound sound);
    }
}