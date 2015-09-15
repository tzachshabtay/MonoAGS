using System;
using AGS.API;

namespace AGS.Engine
{
	public class AGSPlayer : IPlayer
	{
		public AGSPlayer ()
		{
		}

		#region IPlayer implementation

		public ICharacter Character { get; set; }

		#endregion
	}
}

