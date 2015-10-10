using System;

namespace AGS.API
{
	public interface IInventoryFactory
	{
		IInventoryWindow GetInventoryWindow(float width, float height, float itemWidth, float itemHeight, float x, float y, 
			ICharacter character = null, bool addToUi = true);
		IInventoryItem GetInventoryItem(IObject graphics, IObject cursorGraphics, bool playerStartsWithItem = false);
		IInventoryItem GetInventoryItem(string hotspot, string graphicsFile, string cursorFile = null, ILoadImageConfig loadConfig = null,
			bool playerStartsWithItem = false);
	}
}

