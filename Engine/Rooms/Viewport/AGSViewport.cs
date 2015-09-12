using System;
using API;

namespace Engine
{
	public class AGSViewport : IViewport
	{
		public AGSViewport ()
		{
			ScaleX = 1f;
			ScaleY = 1f;
		}

		#region IViewport implementation

		public float X { get; set; }

		public float Y { get; set; }

		public float ScaleX { get; set; }

		public float ScaleY { get; set; }

		public IFollower Follower { get; set; }

		#endregion
	}
}

