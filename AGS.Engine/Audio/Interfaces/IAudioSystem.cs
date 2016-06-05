using System;

namespace AGS.Engine
{
	public interface IAudioSystem
	{
		IAudioListener Listener { get; }

		int AcquireSource();
		void ReleaseSource(int source);

		bool HasErrors();
	}
}

