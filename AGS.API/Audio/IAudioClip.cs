using System;
using System.Threading.Tasks;

namespace AGS.API
{
	public interface IAudioClip : ISoundProperties
	{
		string ID { get; }

		ISound Play(bool shouldLoop = false, ISoundProperties properties = null);
		ISound Play(float volume, bool shouldLoop = false);

		void PlayAndWait(ISoundProperties properties = null);
		void PlayAndWait(float volume);
	}
}

