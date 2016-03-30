using System;
using System.Drawing;

namespace AGS.API
{
	[RequiredComponent(typeof(IAnimationContainer))]
	[RequiredComponent(typeof(IInTree<IObject>))]
	public interface IInventoryWindowComponent : IComponent
	{
		SizeF ItemSize { get; set; }
		ICharacter CharacterToUse { get; set; }
		int TopItem { get; set; }

		void ScrollUp();
		void ScrollDown();

		int ItemsPerRow { get; }
		int RowCount { get; }

	}
}

