using System;

namespace API
{
	public interface IFollower
	{
		bool Enabled { get; set; }
		IObject Target { get; set; }
		IPoint Follow(IPoint point);
	}
}

