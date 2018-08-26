namespace AGS.API
{
    /// <summary>
    /// An audio clip that can be played multiple times during the game.
    /// The audio clip should be loaded from a file using <see cref="IAudioFactory"/>.
    /// You can then set its properties (volume, pitch, panning) which will be used for all sounds played
    /// from this clip. 
    /// 
    /// Note that an audio clip is the template for the sound, whereas <see cref="ISound"/> is the actual
    /// playing sound instance.
    /// </summary>
    [HasFactory(FactoryType = nameof(IAudioFactory), MethodName = nameof(IAudioFactory.LoadAudioClip))]
	public interface IAudioClip : ISoundProperties, ISoundPlayer
	{
        /// <summary>
        /// A unique identifier for the audio clip.
        /// </summary>
        /// <value>The identifier.</value>
		string ID { get; }

        /// <summary>
        /// Gets duration of the sound in seconds.
        /// </summary>
        float Duration { get; }
    }
}

