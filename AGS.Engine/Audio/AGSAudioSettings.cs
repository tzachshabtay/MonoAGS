using System;
using AGS.API;

namespace AGS.Engine
{
	public class AGSAudioSettings : IAudioSettings
	{
		public AGSAudioSettings(ICrossFading roomMusicCrossFading)
		{
			RoomMusicCrossFading = roomMusicCrossFading;
		}

		#region IAudioSettings implementation

		public ICrossFading RoomMusicCrossFading { get; private set; }

		#endregion
	}
}

