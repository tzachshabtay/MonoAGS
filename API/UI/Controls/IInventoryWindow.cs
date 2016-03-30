using System.Drawing;

namespace AGS.API
{
	public interface IInventoryWindow<TControl> : IUIControl<TControl>, IInventoryWindowComponent where TControl : IUIControl<TControl>
	{
	}

	public interface IInventoryWindow : IInventoryWindow<ILabel>
	{}
}

