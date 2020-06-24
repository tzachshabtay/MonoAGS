using AGS.API;

namespace AGS.Engine
{
	public class AGSAudioSettings : IAudioSettings
	{
		private IALAudioSystem _system;

		public AGSAudioSettings(IALAudioSystem system, ICrossFading roomMusicCrossFading)
		{
			_system = system;
			RoomMusicCrossFading = roomMusicCrossFading;
		}

		#region IAudioSettings implementation

		public float MasterVolume
		{
            get => _system.Listener.Volume;
            set => _system.Listener.Volume = value;
        }

		public ICrossFading RoomMusicCrossFading { get; }

		#endregion
	}
}