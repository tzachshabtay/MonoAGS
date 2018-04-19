namespace AGS.API
{
    /// <summary>
    /// A sound modifier can be implemented in order to modify one or more properties of a sound on-the-fly.
    /// For example, the engine has an audio rule (<see cref="IAudioRule"/>) that adds a modifier to all non-speech sounds, which
    /// reduces the volume of those sounds when a speech sound plays.
    /// </summary>
    /// <seealso cref="ISound.SoundModifiers"/>
    public interface ISoundModifier
    {
        /// <summary>
        /// Fires an event when the modifier changes. The sound subscribes to this event
        /// to be notified it that it needs to re-calculate its properties.
        /// </summary>
        /// <value>The on change.</value>
        IBlockingEvent OnChange { get; }

        /// <summary>
        /// Gets the volume after modification.
        /// </summary>
        /// <returns>The volume after modification.</returns>
        /// <param name="volume">Volume before modification.</param>
        float GetVolume(float volume);

        /// <summary>
        /// Gets the pitch after modification.
        /// </summary>
        /// <returns>The pitch after modification.</returns>
        /// <param name="pitch">Pitch before modification.</param>
        float GetPitch(float pitch);

        /// <summary>
        /// Gets the panning after modification.
        /// </summary>
        /// <returns>The panning after modification.</returns>
        /// <param name="panning">Panning before modification.</param>
        float GetPanning(float panning);
    }
}