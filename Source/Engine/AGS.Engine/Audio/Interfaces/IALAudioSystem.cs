namespace AGS.Engine
{
	public interface IALAudioSystem
	{
		IAudioListener Listener { get; }

		int AcquireSource();
		void ReleaseSource(int source);
	}
}