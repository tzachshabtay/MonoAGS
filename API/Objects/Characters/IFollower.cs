using System;
using System.Drawing;

namespace AGS.API
{
	public interface IFollower
	{
		bool Enabled { get; set; }
		Func<IObject> Target { get; set; }
		IPoint Follow(IPoint point, Size roomSize, Size virtualResolution);
	}
}

