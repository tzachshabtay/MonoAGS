using System;

namespace AGS.API
{
	public interface IBlend
	{
		float[] Factors { get; }
		float[] Positions { get; }
	}
}

