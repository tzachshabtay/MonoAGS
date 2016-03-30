using System;

namespace AGS.API
{
	[RequiredComponent(typeof(IInTree<IObject>))]
	public interface IEnabledComponent : IComponent
	{
		bool Enabled { get; set; }
		bool UnderlyingEnabled { get; }
	}
}

