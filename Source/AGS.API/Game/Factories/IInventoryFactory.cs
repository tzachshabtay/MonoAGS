using System.Threading.Tasks;

namespace AGS.API
{
    public interface IInventoryFactory
	{
		IInventoryWindow GetInventoryWindow(string id, float width, float height, float itemWidth, float itemHeight, float x, float y, 
			ICharacter character = null, bool addToUi = true);
		IInventoryWindow GetInventoryWindow(string id, IImage image, float itemWidth, float itemHeight, ICharacter character);

		IInventoryItem GetInventoryItem(IObject graphics, IObject cursorGraphics, bool playerStartsWithItem = false);
		IInventoryItem GetInventoryItem(string hotspot, string graphicsFile, string cursorFile = null, ILoadImageConfig loadConfig = null,
			bool playerStartsWithItem = false);
		Task<IInventoryItem> GetInventoryItemAsync(string hotspot, string graphicsFile, string cursorFile = null, ILoadImageConfig loadConfig = null,
			bool playerStartsWithItem = false);
	}
}

