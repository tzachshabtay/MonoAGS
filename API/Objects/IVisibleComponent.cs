using System;

namespace AGS.API
{
	[RequiredComponent(typeof(IInObjectTree))]
	public interface IVisibleComponent : IComponent
	{
		bool Visible { get; set; }
		bool UnderlyingVisible { get; }
	}
}

