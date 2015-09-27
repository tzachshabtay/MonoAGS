using System;

namespace AGS.API
{
	public interface IOutfit
	{
		IDirectionalAnimation WalkAnimation { get; set; }
		IDirectionalAnimation IdleAnimation { get; set; }
		IDirectionalAnimation SpeakAnimation { get; set; }
	}
}

