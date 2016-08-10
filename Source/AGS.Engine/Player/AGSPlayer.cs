using System;
using AGS.API;

namespace AGS.Engine
{
	public class AGSPlayer : IPlayer
	{
		public AGSPlayer ()
		{
			ApproachStyle = new AGSApproachStyle ();
		}

		#region IPlayer implementation

		public ICharacter Character { get; set; }
		public IApproachStyle ApproachStyle { get; private set; }

		#endregion
	}
}

