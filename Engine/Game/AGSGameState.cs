using System;
using AGS.API;
using System.Collections.Generic;

namespace AGS.Engine
{
	public class AGSGameState : IGameState
	{
		public AGSGameState (IPlayer player, ICustomProperties globalVariables)
		{
			Rooms = new List<IRoom> ();
			UI = new AGSConcurrentHashSet<IObject> ();
			Player = player;
			GlobalVariables = globalVariables;
		}

		#region IGameState implementation

		public IPlayer Player { get; set; }

		public IList<IRoom> Rooms { get; private set; }

		public IConcurrentHashSet<IObject> UI { get; private set; }


		public ICustomProperties GlobalVariables { get; private set; }

		#endregion
	}
}

