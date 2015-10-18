using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AGS.API
{
	public interface IGameState
	{
		IPlayer Player { get; set; }
		IList<IRoom> Rooms { get; }
		IConcurrentHashSet<IObject> UI { get; }

		IDictionary<string, int> Ints { get; }
		IDictionary<string, float> Floats { get; }
		IDictionary<string, string> Strings { get; }

		IList<ICustomSerializable> CustomData { get; }
	}
}

