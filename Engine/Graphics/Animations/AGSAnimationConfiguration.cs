using System;
using API;

namespace Engine
{
	public class AGSAnimationConfiguration : IAnimationConfiguration
	{
		public AGSAnimationConfiguration ()
		{
		}

		#region IAnimationConfiguration implementation

		public LoopingStyle Looping { get; set; }

		public int Loops { get; set; }

		#endregion
	}
}

