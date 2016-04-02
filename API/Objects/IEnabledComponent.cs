using System;

namespace AGS.API
{
	[RequiredComponent(typeof(IInObjectTree))]
	public interface IEnabledComponent : IComponent
	{
		bool Enabled { get; set; }
		bool UnderlyingEnabled { get; }
	}
}

