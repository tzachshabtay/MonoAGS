using System;
using AGS.API;

namespace AGS.Engine
{
	public interface IAudioListener
	{
		float Volume { get; set; }
		ILocation Location { get; set; }
	}
}

