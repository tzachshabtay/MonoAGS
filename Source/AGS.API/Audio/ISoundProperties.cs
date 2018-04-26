namespace AGS.API
{
    /// <summary>
    /// Allows to read or adjust the properties (volume, pitch, gain) of a sound/clip.
    /// </summary>
	public interface ISoundProperties
	{
        /// <summary>
		/// Gets or sets the volume.
		/// The minimum volume is 0 (no volume).
		/// The maximum volume is 1 (also the default), though some sound cards may actually support increasing the volume.
        /// 
        /// Note that this is not necessarily the actual volume in which the sound will be played, as it might be modified by other systems 
        /// (like reducing volume for background sounds when speech sound is playing). To query the real volume, <see cref=" ISound.RealVolume"/>.
		/// </summary>
        /// <seealso cref="ISound.SoundModifiers"/>
		/// <value>The volume.</value>
		float Volume { get; set; }

		/// <summary>
		/// Gets or sets the pitch multiplier.
		/// The default is 1 (no pitch change). 
		/// The minimum is 0.0001 (or, to be accurate, more than 0).
        /// 
        /// Note that this is not necessarily the actual pitch in which the sound will be played, as it might be modified by other systems 
        /// To query the real volume, <see cref=" ISound.RealPitch"/>.
        /// </summary>
        /// <seealso cref="ISound.SoundModifiers"/>
		/// <value>The pitch.</value>
		float Pitch { get; set; }

		/// <summary>
		/// Gets or sets the panning the sound is coming from.
		/// The valid range is -1..1 
		/// -1 will play the sound on the left speaker.
		/// 1 will play the sound on the right speaker.
		/// 0 will play the sound equally on both speakers.
		/// Values in between will be interpolated accordingly.
		/// Note: Panning can only work on Mono sounds (it will not work for Stereo sounds).
        ///
        /// Also note that this is not necessarily the actual panning in which the sound will be played, as it might be modified by other systems 
        /// To query the real volume, <see cref=" ISound.RealPanning"/>.
        /// </summary>
        /// <seealso cref="ISound.SoundModifiers"/>
		/// <value>The location.</value>
		float Panning { get; set; }

        /// <summary>
        /// Allows adding a tag to an audio clip/sound, which can be used by the engine or the game to modify this or other sounds accordingly.
        /// For example, if the sound is tagged as "Speech", the engine has an opt-in rule (ReduceWhenSpeechAudioRule) which reduces the volume of all 
        /// playing sounds that are not tagged as "Speech".
        /// Common tags, other than "Speech" might be "Music" and "Sounds", but you can be creative with your custom tags (like: "In a cave") and add rules (<see cref="IAudioSystem.AudioRules"/>)
        /// that modify the sounds based on those tags.
        /// </summary>
        /// <value>The tags.</value>
        IConcurrentHashSet<string> Tags { get; }
	}
}