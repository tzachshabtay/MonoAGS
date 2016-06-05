using System;

namespace AGS.API
{
	public interface IAudioSettings
	{
		/// <summary>
		/// Gets or sets the master volume.
		/// The valid range is between 0 (sound is muted) and 1 (no change is made to the volume).
		/// Default value is 0.5.
		/// </summary>
		/// <value>The master volume.</value>
		float MasterVolume { get; set; }

		/// <summary>
		/// Gets the settings for cross fading music clips when transitioning between rooms.
		/// </summary>
		/// <value>The room music cross fading.</value>
		ICrossFading RoomMusicCrossFading { get; }
	}
}

