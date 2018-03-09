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
		/// </summary>
		/// <value>The volume.</value>
		float Volume { get; set; }

		/// <summary>
		/// Gets or sets the pitch multiplier.
		/// The default is 1 (no pitch change). 
		/// The minimum is 0.0001 (or, to be accurate, more than 0).
		/// </summary>
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
		/// </summary>
		/// <value>The location.</value>
		float Panning { get; set; }
	}
}

