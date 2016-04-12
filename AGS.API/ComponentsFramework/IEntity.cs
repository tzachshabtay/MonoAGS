using System;

namespace AGS.API
{
	public interface IEntity : IComponentsCollection
	{
		string ID { get; }
	}
}

