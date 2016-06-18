using System;
using AGS.API;
using Autofac;

using System.Diagnostics;
using System.Threading.Tasks;

namespace AGS.Engine
{
	public class AGSInventoryFactory : IInventoryFactory
	{
		private IContainer _resolver;
		private IGameState _gameState;
		private IGraphicsFactory _graphics;
		private IObjectFactory _object;

		public AGSInventoryFactory(IContainer resolver, IGameState gameState, IGraphicsFactory graphics, IObjectFactory obj)
		{
			_resolver = resolver;
			_gameState = gameState;
			_graphics = graphics;
			_object = obj;
		}

		public IInventoryWindow GetInventoryWindow(string id, float width, float height, float itemWidth, float itemHeight, float x, float y,
			ICharacter character = null, bool addToUi = true)
		{
			IInventoryWindow inventory = GetInventoryWindow(id, new EmptyImage(width, height), itemWidth, itemHeight, character);
			inventory.X = x;
			inventory.Y = y;

			if (addToUi)
				_gameState.UI.Add(inventory);

			return inventory;
		}

		public IInventoryWindow GetInventoryWindow(string id, IImage image, float itemWidth, float itemHeight, ICharacter character)
		{
			TypedParameter idParam = new TypedParameter (typeof(string), id);
			TypedParameter imageParam = new TypedParameter (typeof(IImage), image);
			IInventoryWindow inventory = _resolver.Resolve<IInventoryWindow>(idParam, imageParam);
			inventory.Tint =  Colors.Transparent;
			inventory.ItemSize = new AGS.API.SizeF (itemWidth, itemHeight);
			inventory.CharacterToUse = character ?? _resolver.Resolve<IPlayer>().Character;
			return inventory;
		}

		public IInventoryItem GetInventoryItem(IObject graphics, IObject cursorGraphics, bool playerStartsWithItem = false)
		{
			IInventoryItem item = _resolver.Resolve<IInventoryItem>();
			item.Graphics = graphics;
			item.CursorGraphics = cursorGraphics;

			if (playerStartsWithItem)
			{
				IPlayer player = _resolver.Resolve<IPlayer>();
				if (player.Character == null)
				{
					Debug.WriteLine(string.Format("Character was not assigned to player yet, cannot add inventory item :{0}", graphics.ToString()));
				}

				else player.Character.Inventory.Items.Add(item);
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
			IObject graphics = _object.GetObject (string.Format ("{0}(inventory item)", hotspot ?? ""));
			graphics.Image = graphicsImage;
			graphics.RenderLayer = AGSLayers.UI;
			graphics.IgnoreViewport = true;
			graphics.IgnoreScalingArea = true;
			graphics.Anchor = new PointF (0.5f, 0.5f);
			graphics.Hotspot = hotspot;

			IObject cursor = _object.GetObject (string.Format ("{0}(inventory item cursor)", hotspot ?? ""));
			cursor.Image = cursorImage;

			return GetInventoryItem (graphics, cursor, playerStartsWithItem);
		}
	}
}

