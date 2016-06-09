using System;

namespace AGS.API
{
	public interface ISoundPlayer
	{
		ISound Play(bool shouldLoop = false, ISoundProperties properties = null);
		ISound Play(float volume, bool shouldLoop = false);

		void PlayAndWait(ISoundProperties properties = null);
		void PlayAndWait(float volume);
	}
}

