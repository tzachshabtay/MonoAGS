using System;
using AGS.API;

namespace AGS.Engine
{
	public interface IAudioListener
	{
		float Volume { get; set; }
		Position Position { get; set; }
	}
}

