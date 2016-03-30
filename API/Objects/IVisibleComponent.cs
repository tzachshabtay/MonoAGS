using System;

namespace AGS.API
{
	[RequiredComponent(typeof(IInTree<IObject>))]
	public interface IVisibleComponent : IComponent
	{
		bool Visible { get; set; }
		bool UnderlyingVisible { get; }
	}
}

