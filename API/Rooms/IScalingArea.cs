using System;

namespace API
{
	public interface IScalingArea : IArea
	{
		float MinScaling { get; set; }
		float MaxScaling { get; set; }
	}
}

