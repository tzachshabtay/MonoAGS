using System;

namespace AGS.API
{
	[RequiredComponent(typeof(IScaleComponent))]
	[RequiredComponent(typeof(IInObjectTree))]
	public interface IInventoryWindowComponent : IComponent
	{
		SizeF ItemSize { get; set; }
        IInventory Inventory { get; set; }
		int TopItem { get; set; }

		void ScrollUp();
		void ScrollDown();

		int ItemsPerRow { get; }
		int RowCount { get; }

	}
}

