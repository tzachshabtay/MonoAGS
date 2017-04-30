using System.Threading.Tasks;

namespace AGS.API
{
    /// <summary>
    /// A factory for creating inventory items and inventory windows.
    /// </summary>
    public interface IInventoryFactory
	{
        /// <summary>
        /// Creates a new inventory window
        /// </summary>
        /// <returns>The inventory window.</returns>
        /// <param name="id">A unique identifier for the window.</param>
        /// <param name="width">The width of the window.</param>
        /// <param name="height">The height of the window.</param>
        /// <param name="itemWidth">The width which will be allocated for each inventory item.</param>
        /// <param name="itemHeight">The height which will be allocated for each inventory item.</param>
        /// <param name="x">The x coordinate for the inventory window.</param>
        /// <param name="y">The y coordinate for the inventory window.</param>
        /// <param name="inventory">The inventory that the window will show on screen (usually the player's inventory).</param>
        /// <param name="addToUi">If set to <c>true</c> add to the window to the GUI list of the game.</param>
		IInventoryWindow GetInventoryWindow(string id, float width, float height, float itemWidth, float itemHeight, float x, float y, 
			IInventory inventory = null, bool addToUi = true);

        /// <summary>
        /// Creates a new inventory window
        /// </summary>
        /// <returns>The inventory window.</returns>
        /// <param name="id">A unique identifier for the window.</param>
        /// <param name="image">A background image for the inventory window.</param>
        /// <param name="itemWidth">The width which will be allocated for each inventory item.</param>
        /// <param name="itemHeight">The height which will be allocated for each inventory item.</param>
        /// <param name="inventory">The inventory that the window will show on screen (usually the player's inventory).</param>
        IInventoryWindow GetInventoryWindow(string id, IImage image, float itemWidth, float itemHeight, IInventory inventory);

        /// <summary>
        /// Creates a new inventory item.
        /// </summary>
        /// <returns>The inventory item.</returns>
        /// <param name="graphics">Graphics object for the item.</param>
        /// <param name="cursorGraphics">When the item is selected for using on other objects, the cursor will be using this graphics .</param>
        /// <param name="playerStartsWithItem">If set to <c>true</c> player will have the item at the beginning of the game.</param>
		IInventoryItem GetInventoryItem(IObject graphics, IObject cursorGraphics, bool playerStartsWithItem = false);

        /// <summary>
        /// Creates a new inventory item
        /// </summary>
        /// <returns>The inventory item.</returns>
        /// <param name="hotspot">The hotspot text (which will be shown when hovering the item if a hotspot label is in the game).</param>
        /// <param name="graphicsFile">Graphics file to load the item graphics file.</param>
        /// <param name="cursorFile">Cursor file to load the graphics for the cursor when the item is selected.</param>
        /// <param name="loadConfig">A load configuration for the images.</param>
        /// <param name="playerStartsWithItem">If set to <c>true</c> player will have the item at the beginning of the game.</param>
		IInventoryItem GetInventoryItem(string hotspot, string graphicsFile, string cursorFile = null, ILoadImageConfig loadConfig = null,
			bool playerStartsWithItem = false);

        /// <summary>
        /// Creates a new inventory item asynchronously
        /// </summary>
        /// <returns>The inventory item.</returns>
        /// <param name="hotspot">The hotspot text (which will be shown when hovering the item if a hotspot label is in the game).</param>
        /// <param name="graphicsFile">Graphics file to load the item graphics file.</param>
        /// <param name="cursorFile">Cursor file to load the graphics for the cursor when the item is selected.</param>
        /// <param name="loadConfig">A load configuration for the images.</param>
        /// <param name="playerStartsWithItem">If set to <c>true</c> player will have the item at the beginning of the game.</param>
		Task<IInventoryItem> GetInventoryItemAsync(string hotspot, string graphicsFile, string cursorFile = null, ILoadImageConfig loadConfig = null,
			bool playerStartsWithItem = false);
	}
}

