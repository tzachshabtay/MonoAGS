using System;

namespace AGS.API
{
	public interface ICutscene
	{
		bool IsSkipping { get; }
		bool IsRunning { get; }

		void Start();
		void End();
	}
}

