using System;
using AGS.API;
using AGS.Engine;

namespace DemoGame
{
	public class TopBar
	{
		private const string _baseFolder = "../../Assets/Gui/Buttons/";
		private IPanel _panel;
		private readonly RotatingCursorScheme _scheme;
		private InventoryPanel _invPanel;
		private OptionsPanel _optionsPanel;

		private IInventoryItem _lastInventoryItem;
		private IObject _inventoryItemIcon;
		private IPlayer _player;

		public TopBar(RotatingCursorScheme scheme, InventoryPanel invPanel, OptionsPanel optionsPanel)
		{
			_scheme = scheme;
			_invPanel = invPanel;
			_optionsPanel = optionsPanel;
		}

		public void Load(IGame game)
		{
			IGameFactory factory = game.Factory;
			_player = game.State.Player;
			_panel = factory.UI.GetPanel("../../Assets/Gui/DialogBox/toolbar.bmp", 0f, 180f);

			loadButton(factory, "walk/", 0f, RotatingCursorScheme.WALK_MODE);
			loadButton(factory, "hand/", 20f, RotatingCursorScheme.INTERACT_MODE);
			loadButton(factory, "eye/", 40f, RotatingCursorScheme.LOOK_MODE);
			loadButton(factory, "talk/", 60f, MouseCursors.TALK_MODE);
			IButton invButton = loadButton(factory, "inventory/", 80f);
			IButton activeInvButton = loadButton(factory, "activeInventory/", 100f, RotatingCursorScheme.INVENTORY_MODE);
			loadButton(factory, "help/", 280f);
			IButton optionsButton = loadButton(factory, "settings/", 300f);

			invButton.Events.MouseClicked.Subscribe((sender, e) => _invPanel.Show());
			optionsButton.Events.MouseClicked.Subscribe((sender, e) => _optionsPanel.Show());

			game.Events.OnRepeatedlyExecute.Subscribe(onRepeatedlyExecute);
			_inventoryItemIcon = factory.Object.GetObject();
			_inventoryItemIcon.X = 10f;
			_inventoryItemIcon.Y = 10f;
			_inventoryItemIcon.Anchor = new AGSPoint (0f, 0f);
			_inventoryItemIcon.TreeNode.SetParent(activeInvButton.TreeNode);
			_inventoryItemIcon.RenderLayer = _panel.RenderLayer;
			game.State.UI.Add(_inventoryItemIcon);
		}

		private IButton loadButton(IGameFactory factory, string folder, float x, string mode = null)
		{
			folder = _baseFolder + folder;
			IButton button = factory.UI.GetButton(folder + "normal.bmp", folder + "hovered.bmp", folder + "pushed.bmp", x, 0f);
			button.TreeNode.SetParent(_panel.TreeNode);
			if (mode != null)
			{
				button.Events.MouseClicked.Subscribe((sender, e) => _scheme.CurrentMode = mode);
			}
			return button;
		}

		private void onRepeatedlyExecute(object sender, AGSEventArgs args)
		{
			if (_lastInventoryItem == _player.Character.Inventory.ActiveItem) return;

			_lastInventoryItem = _player.Character.Inventory.ActiveItem;

			if (_lastInventoryItem != null)
			{
				_inventoryItemIcon.Image = _lastInventoryItem.CursorGraphics.Image;
				_inventoryItemIcon.Animation.Sprite.Anchor = new AGSPoint (0.5f, 0.5f);
				_inventoryItemIcon.ScaleTo(15f, 15f);
			}
			_inventoryItemIcon.Visible = (_lastInventoryItem != null);
		}
	}
}

