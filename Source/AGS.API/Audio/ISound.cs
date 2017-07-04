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
        /// <code>
        /// private async Task onMusicBoxInteract(object args)
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
		/// Gets or sets the seek (position within the sound) in seconds.
		/// </summary>
		/// <value>The seek.</value>
		float Seek { get; set; }

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

