using System;
using API;

namespace Engine
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

