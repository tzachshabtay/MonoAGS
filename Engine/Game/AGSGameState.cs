using System;
using API;
using System.Collections.Generic;

namespace Engine
{
	public class AGSGameState : IGameState
	{
		public AGSGameState (IPlayer player)
		{
			Rooms = new List<IRoom> ();
			UI = new List<IObject> ();
			Ints = new Dictionary<string, int> ();
			Floats = new Dictionary<string, float> ();
			Strings = new Dictionary<string, string> ();
			CustomData = new List<ICustomSerializable> ();
			Player = player;
		}

		#region IGameState implementation

		public IPlayer Player { get; set; }

		public IList<IRoom> Rooms { get; private set; }

		public IList<IObject> UI { get; private set; }

		public IDictionary<string, int> Ints { get; private set; }

		public IDictionary<string, float> Floats { get; private set; }

		public IDictionary<string, string> Strings { get; private set; }

		public IList<ICustomSerializable> CustomData { get; private set; }

		#endregion
	}
}

