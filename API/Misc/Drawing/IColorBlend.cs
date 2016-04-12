using System;

namespace AGS.API
{
	public interface IColorBlend
	{
		IColor[] Colors { get; }
		float[] Positions { get; }
	}
}

