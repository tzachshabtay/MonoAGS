using System.Collections.ObjectModel;

namespace AGS.API
{
    /// <summary>
    /// Allows to play a sound.
    /// </summary>
	public interface ISoundPlayer
	{
        /// <summary>
        /// Plays a sound.
        /// </summary>
        /// <param name="shouldLoop">If set to <c>true</c> should loop.</param>
        /// <param name="properties">Properties.</param>
        /// <example>
        /// In order to play a sound and then asynchronously wait for it to finish:
        /// <code language="lang-csharp">
        /// private async Task onSpecificTrigger()
        /// {
        ///     ISound sound = myAudioClip.Play();
        ///     doStuffWhileSoundIsPlaying(); //This happens in parallel to the playing sound
        ///     await sound.Completed;
        ///     doStuffAfterSoundFinishedPlaying();
        /// }
        /// </code>
        /// </example>
		ISound Play(bool shouldLoop = false, ISoundProperties properties = null);

        /// <summary>
        /// Plays a sound
        /// </summary>
        /// <param name="volume">Volume (allowed values: 0-1, see ISoundPropertie).</param>
        /// <param name="shouldLoop">If set to <c>true</c> should loop.</param>
		ISound Play(float volume, bool shouldLoop = false);

        /// <summary>
        /// Plays a sound and blocks until the sound is completed.
        /// </summary>
        /// <param name="properties">Properties.</param>
		void PlayAndWait(ISoundProperties properties = null);
        /// <summary>
        /// Plays a sound and blocks until the sound is completed.
        /// </summary>
        /// <param name="volume">Volume (allowed values: 0-1, see ISoundPropertie).</param>
		void PlayAndWait(float volume);

        /// <summary>
        /// Gets a value indicating whether this <see cref="T:AGS.API.ISoundPlayer"/> is playing a sound (or more than 1 sound).
        /// </summary>
        /// <value><c>true</c> if is playing; otherwise, <c>false</c>.</value>
        bool IsPlaying { get; }

        /// <summary>
        /// Gets a list of currently playing sounds for this audio clip.
        /// </summary>
        /// <value>The list of currently playing sounds.</value>
        ReadOnlyCollection<ISound> CurrentlyPlayingSounds { get; }

        /// <summary>
        /// An event which is sent when a new sound starts playing.
        /// </summary>
        /// <value>The event.</value>
        IBlockingEvent<ISound> OnSoundStarted { get; }

        /// <summary>
        /// An event which is sent when an existing sound has completed playing.
        /// </summary>
        /// <value>The event.</value>
        IBlockingEvent<ISound> OnSoundCompleted { get; }
	}
}