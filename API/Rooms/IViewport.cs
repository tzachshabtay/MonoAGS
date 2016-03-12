using System;

namespace AGS.API
{
	public interface IViewport
	{
		float X { get; set; }
		float Y { get; set; }

		float ScaleX { get; set; }
		float ScaleY { get; set; }

		ICamera Camera { get; set; }
	}
}

