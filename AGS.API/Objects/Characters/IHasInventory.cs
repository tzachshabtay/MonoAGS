using System;

namespace AGS.API
{
	public interface IHasInventory : IComponent
	{
		IInventory Inventory { get; set; }
	}
}

