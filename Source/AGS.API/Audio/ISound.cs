using System.Threading.Tasks;

namespace AGS.API
{
    /// <summary>
    /// An instance of a playing sound.
    /// You'll usually use an <see cref="IAudioClip"/> to play a sound, and control the playing sound from this interface. 
    /// </summary>
	public interface ISound : ISoundProperties
	{
        /// <summary>
        /// An internal id for the source that is playing the sound. This can be used if low-level OpenAL access is needed,
        /// but for regular uses can be ignored.
        /// </summary>
        /// <value>The source identifier.</value>
		int SourceID { get; }

        /// <summary>
        /// Is the sound a valid sound and expected to play properly, or is there some audio problem and this is a dummy object?
        /// </summary>
        bool IsValid { get; }

        /// <summary>
        /// Gets duration of the sound in seconds.
        /// </summary>
        float Duration { get; }

        /// <summary>
        /// Is this <see cref="T:AGS.API.ISound"/> paused?
        /// </summary>
        /// <value><c>true</c> if is paused; otherwise, <c>false</c>.</value>
		bool IsPaused { get; }

        /// <summary>
        /// Is this <see cref="T:AGS.API.ISound"/> looping (i.e running in an endless loop)?
        /// </summary>
        /// <value><c>true</c> if is looping; otherwise, <c>false</c>.</value>
		bool IsLooping { get; }

        /// <summary>
        /// Has this <see cref="T:AGS.API.ISound"/> completed playing?
        /// </summary>
        /// <value><c>true</c> if has completed; otherwise, <c>false</c>.</value>
		bool HasCompleted { get; }

        /// <summary>
        /// A task that can be used to await a playing sound asynchronously until it completes (do not use on a looping sound).
        /// </summary>
        /// <example>
        /// <code language="lang-csharp">
        /// private async Task onMusicBoxInteract()
        /// {
        ///     ISound sound = musicBoxAudioClip.Play();
        ///     await cHero.SayAsync("The music box is currently playing a song..."); //We didn't wait for the sound to complete yet, so the character is speaking in parallel
        ///     await sound.Completed;
        ///     await cHero.SayAsync("What a nice song that was!");
        /// }
        /// </code>
        /// </example>
        /// <value>The completed.</value>
		Task Completed { get; }

		/// <summary>
		/// Gets or sets the playback position within the sound in seconds.
		/// </summary>
		/// <value>The position.</value>
		float Position { get; set; }

        /// <summary>
        /// Gets the real volume of the sound (after it has been modified by all the existing sound modifiers).
        /// </summary>
        /// <seealso cref="SoundModifiers"/>
        /// <seealso cref="ISoundProperties.Volume"/>
        /// <value>The real volume.</value>
        float RealVolume { get; }

        /// <summary>
        /// Gets the real pitch of the sound (after it has been modified by all the existing sound modifiers).
        /// </summary>
        /// <seealso cref="SoundModifiers"/>
        /// <seealso cref="ISoundProperties.Pitch"/>
        /// <value>The real pitch.</value>
        float RealPitch { get; }

        /// <summary>
        /// Gets the real panning of the sound (after it has been modified by all the existing sound modifiers).
        /// </summary>
        /// <seealso cref="SoundModifiers"/>
        /// <seealso cref="ISoundProperties.Panning"/>
        /// <value>The real panning.</value>
        float RealPanning { get; }

        /// <summary>
        /// A list of modifiers for sound properties. The sound modifiers act on the existing sound properties that you set (volume, panning, pitch),
        /// and modify them on-the-fly (for example, the engine has a rule where the volume of non-speech sounds is reduced when speech is playing).
        /// </summary>
        /// <seealso cref="ISoundProperties.Volume"/>
        /// <seealso cref="ISoundProperties.Pitch"/>
        /// <seealso cref="ISoundProperties.Panning"/>
		/// <seealso cref="RealVolume"/>
        /// <seealso cref="RealPitch"/>
        /// <seealso cref="RealPanning"/>
        /// <value>The list of sound modifiers.</value>
        IAGSBindingList<ISoundModifier> SoundModifiers { get; }

        /// <summary>
        /// Pause this sound.
        /// </summary>
		void Pause();

        /// <summary>
        /// Resume this sound (if the sound is already playing, or already completed, then it does nothing).
        /// </summary>
		void Resume();

        /// <summary>
        /// Rewind this sound to the beginning (if the sound has already completed, then it does nothing).
        /// </summary>
		void Rewind();

        /// <summary>
        /// Stop this sound (if the sound has already completed, then it does nothing).
        /// </summary>
		void Stop();
	}
}