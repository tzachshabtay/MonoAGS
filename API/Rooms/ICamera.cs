using System;
using System.Drawing;

namespace AGS.API
{
	public interface ICamera
	{
		bool Enabled { get; set; }
		Func<IObject> Target { get; set; }
		void Tick(IViewport viewport, Size roomSize, Size virtualResolution, bool resetPosition);
	}
}

