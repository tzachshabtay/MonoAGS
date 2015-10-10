using System;
using AGS.API;
using Autofac;
using System.Drawing;
using System.Diagnostics;

namespace AGS.Engine
{
	public class AGSInventoryFactory : IInventoryFactory
	{
		private IContainer _resolver;
		private IGameState _gameState;
		private IGraphicsFactory _graphics;
		private IObjectFactory _object;
		private IUIFactory _ui;

		public AGSInventoryFactory(IContainer resolver, IGameState gameState, IGraphicsFactory graphics, IObjectFactory obj,
			IUIFactory ui)
		{
			_resolver = resolver;
			_gameState = gameState;
			_graphics = graphics;
			_object = obj;
			_ui = ui;
		}

		public IInventoryWindow GetInventoryWindow(float width, float height, float itemWidth, float itemHeight, float x, float y,
			ICharacter character = null, bool addToUi = true)
		{
			IPanel panel = _ui.GetPanel(width, height, x, y, false);
			TypedParameter parameter = new TypedParameter (typeof(IPanel), panel);
			IInventoryWindow inventory = _resolver.Resolve<IInventoryWindow>(parameter);
			inventory.Tint = Color.Transparent;
			inventory.ItemSize = new SizeF (itemWidth, itemHeight);
			inventory.CharacterToUse = character ?? _resolver.Resolve<IPlayer>().Character;

			if (addToUi)
				_gameState.UI.Add(inventory);

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
					Debug.WriteLine("Character was not assigned to player yet, cannot add inventory item", graphics.ToString());
				}
				else player.Character.Inventory.Items.Add(item);
			}
			return item;
		}

		public IInventoryItem GetInventoryItem(string hotspot, string graphicsFile, string cursorFile = null, 
			ILoadImageConfig loadConfig = null, bool playerStartsWithItem = false)
		{
			IObject graphics = _object.GetObject();
			graphics.Image = _graphics.LoadImage(graphicsFile, loadConfig);
			graphics.RenderLayer = AGSLayers.UI;
			graphics.IgnoreViewport = true;
			graphics.IgnoreScalingArea = true;
			graphics.Anchor = new AGSPoint (0.5f, 0.5f);
			graphics.Hotspot = hotspot;

			IObject cursor = _object.GetObject();
			cursor.Image = cursorFile == null ? graphics.Image : _graphics.LoadImage(cursorFile, loadConfig);

			return GetInventoryItem(graphics, cursor, playerStartsWithItem);
		}
	}
}

