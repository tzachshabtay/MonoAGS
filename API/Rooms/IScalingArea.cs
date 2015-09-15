using System;

namespace AGS.API
{
	public interface IScalingArea : IArea
	{
		float MinScaling { get; set; }
		float MaxScaling { get; set; }
	}
}

