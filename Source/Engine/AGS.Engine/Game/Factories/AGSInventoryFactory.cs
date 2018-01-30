using AGS.API;
using Autofac;
using System.Threading.Tasks;

namespace AGS.Engine
{
	public class AGSInventoryFactory : IInventoryFactory
	{
        private readonly Resolver _resolver;
		private readonly IGameState _gameState;
		private readonly IGraphicsFactory _graphics;
		private readonly IObjectFactory _object;

		public AGSInventoryFactory(Resolver resolver, IGameState gameState, IGraphicsFactory graphics, IObjectFactory obj)
		{
			_resolver = resolver;
			_gameState = gameState;
			_graphics = graphics;
			_object = obj;
		}

		public IInventoryWindow GetInventoryWindow(string id, float width, float height, float itemWidth, float itemHeight, float x, float y,
			IInventory inventory = null, bool addToUi = true)
		{
            IInventoryWindow inventoryWindow = GetInventoryWindow(id, new EmptyImage(width, height), itemWidth, itemHeight, inventory);
			inventoryWindow.X = x;
			inventoryWindow.Y = y;

			if (addToUi)
				_gameState.UI.Add(inventoryWindow);

			return inventoryWindow;
		}

		public IInventoryWindow GetInventoryWindow(string id, IImage image, float itemWidth, float itemHeight, IInventory inventory)
		{
			TypedParameter idParam = new TypedParameter (typeof(string), id);
			TypedParameter imageParam = new TypedParameter (typeof(IImage), image);
			IInventoryWindow inventoryWindow = _resolver.Container.Resolve<IInventoryWindow>(idParam, imageParam);
			inventoryWindow.Tint =  Colors.Transparent;
			inventoryWindow.ItemSize = new SizeF (itemWidth, itemHeight);
            inventoryWindow.Inventory = inventory ?? _gameState.Player.Inventory;
			return inventoryWindow;
		}

		public IInventoryItem GetInventoryItem(IObject graphics, IObject cursorGraphics, bool playerStartsWithItem = false)
		{
			IInventoryItem item = _resolver.Container.Resolve<IInventoryItem>();
			item.Graphics = graphics;
			item.CursorGraphics = cursorGraphics;

			if (playerStartsWithItem)
			{
                _gameState.Player.Inventory.Items.Add(item);
			}
			return item;
		}

		public IInventoryItem GetInventoryItem(string hotspot, string graphicsFile, string cursorFile = null, 
			ILoadImageConfig loadConfig = null, bool playerStartsWithItem = false)
		{
			var graphicsImage = _graphics.LoadImage(graphicsFile, loadConfig);
			var cursorImage = cursorFile == null ? graphicsImage : _graphics.LoadImage(cursorFile, loadConfig);
			return getInventoryItem (hotspot, graphicsImage, cursorImage, playerStartsWithItem);
		}

		public async Task<IInventoryItem> GetInventoryItemAsync(string hotspot, string graphicsFile, string cursorFile = null,
			ILoadImageConfig loadConfig = null, bool playerStartsWithItem = false)
		{
			var graphicsImage = await _graphics.LoadImageAsync (graphicsFile, loadConfig);
			var cursorImage = cursorFile == null ? graphicsImage : await _graphics.LoadImageAsync(cursorFile, loadConfig);
			return getInventoryItem(hotspot, graphicsImage, cursorImage, playerStartsWithItem);
		}

		private IInventoryItem getInventoryItem(string hotspot, IImage graphicsImage, IImage cursorImage, bool playerStartsWithItem = false)
		{
			IObject graphics = _object.GetObject ($"{hotspot ?? ""}(inventory item)");
			graphics.Image = graphicsImage;
			graphics.RenderLayer = AGSLayers.UI;
			graphics.IgnoreViewport = true;
			graphics.IgnoreScalingArea = true;
			graphics.Pivot = new PointF (0.5f, 0.5f);
			graphics.DisplayName = hotspot;

			IObject cursor = _object.GetObject ($"{hotspot ?? ""}(inventory item cursor)");
			cursor.Image = cursorImage;
            cursor.IgnoreViewport = true;
            cursor.IgnoreScalingArea = true;
            cursor.Pivot = new PointF(0f, 1f);

            return GetInventoryItem (graphics, cursor, playerStartsWithItem);
		}
	}
}

