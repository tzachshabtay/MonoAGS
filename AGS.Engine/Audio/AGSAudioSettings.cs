using System;
using AGS.API;

namespace AGS.Engine
{
	public class AGSAudioSettings : IAudioSettings
	{
		private IAudioSystem _system;

		public AGSAudioSettings(IAudioSystem system, ICrossFading roomMusicCrossFading)
		{
			_system = system;
			RoomMusicCrossFading = roomMusicCrossFading;
		}

		#region IAudioSettings implementation

		public float MasterVolume
		{ 
			get { return _system.Listener.Volume; }
			set { _system.Listener.Volume = value; }
		}

		public ICrossFading RoomMusicCrossFading { get; private set; }

		#endregion
	}
}

