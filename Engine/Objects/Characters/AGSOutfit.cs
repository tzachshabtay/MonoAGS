using System;
using AGS.API;

namespace AGS.Engine
{
	public class AGSOutfit : IOutfit
	{
		public AGSOutfit()
		{
		}

		#region IOutfit implementation

		public IDirectionalAnimation WalkAnimation { get; set; }

		public IDirectionalAnimation IdleAnimation { get; set; }

		public IDirectionalAnimation SpeakAnimation { get; set; }

		#endregion
	}
}

