using System;
using AGS.API;

namespace AGS.Engine
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

