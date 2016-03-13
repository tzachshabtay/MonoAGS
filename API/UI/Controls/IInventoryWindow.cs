using System.Drawing;

namespace AGS.API
{
    public interface IInventoryWindow<TControl> : IUIControl<TControl> where TControl : IUIControl<TControl>
	{
		SizeF ItemSize { get; set; }
		ICharacter CharacterToUse { get; set; }
		int TopItem { get; set; }

		void ScrollUp();
		void ScrollDown();

		int ItemsPerRow { get; }
		int RowCount { get; }
	}

	public interface IInventoryWindow : IInventoryWindow<ILabel>
	{}
}

