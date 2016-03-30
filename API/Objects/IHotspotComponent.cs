using System;

namespace AGS.API
{
	public interface IHotspotComponent : IComponent
	{
		IInteractions Interactions { get; }
		IPoint WalkPoint { get; set; }
		string Hotspot { get; set; }
	}
}

