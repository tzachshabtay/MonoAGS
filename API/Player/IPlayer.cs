using System;

namespace AGS.API
{
	public interface IPlayer
	{
		ICharacter Character { get; set; }

		IApproachStyle ApproachStyle { get; }
	}
}

