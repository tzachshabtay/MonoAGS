using AGS.API;

namespace AGS.Engine
{
	public class AGSAnimationConfiguration : IAnimationConfiguration
	{
		public AGSAnimationConfiguration ()
		{
            DelayBetweenFrames = 5;
		}

		#region IAnimationConfiguration implementation

		public LoopingStyle Looping { get; set; }

		public int Loops { get; set; }

        public int DelayBetweenFrames { get; set; }

		#endregion
	}
}

