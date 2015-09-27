using System;
using AGS.API;

namespace AGS.Engine
{
	public interface IAGSFaceDirectionBehavior : IFaceDirectionBehavior
	{
		new IDirectionalAnimation CurrentDirectionalAnimation { get; set; }
	}
}

