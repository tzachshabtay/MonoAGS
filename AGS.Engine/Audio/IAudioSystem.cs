using System;

namespace AGS.Engine
{
	public interface IAudioSystem
	{
		int AcquireSource();
		void ReleaseSource(int source);

		bool HasErrors();
	}
}

