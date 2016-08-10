using System;

namespace AGS.API
{
	public interface IHotspotComponent : IComponent
	{
		IInteractions Interactions { get; }
		PointF? WalkPoint { get; set; }
		string Hotspot { get; set; }
	}
}

