using System;
using AGS.API;
using AGS.Engine;

namespace DemoGame
{
	public class InventoryPanel
	{
		private const string _baseFolder = "../../Assets/Gui/Buttons/";
		private IPanel _panel;
		private readonly RotatingCursorScheme _scheme;
		private string _lastMode;
		private IGame _game;
		private IInventoryWindow _invWindow;

		public InventoryPanel(RotatingCursorScheme scheme)
		{
			_scheme = scheme;
		}

		public void Load(IGame game)
		{
			_game = game;
			_game.Events.OnSavedGameLoad.Subscribe((sender, e) => onSaveGameLoaded());
			IGameFactory factory = game.Factory;

			_panel = factory.UI.GetPanel("Inventory Panel", "../../Assets/Gui/DialogBox/inventory.bmp", 160f, 100f);
			_panel.Anchor = new AGSPoint (0.5f, 0.5f);
			_panel.Visible = false;

			loadButton("Inventory Look Button", factory, "magnify/", 5f, RotatingCursorScheme.LOOK_MODE);
			loadButton("Invntory Point Button", factory, "upLeft/", 27f, MouseCursors.POINT_MODE);
			IButton okButton = loadButton("Inventory Ok Button", factory, "ok/", 49f);
			IButton upButton = loadButton("Inventory Up Button", factory, "up/", 93f);
			IButton downButton = loadButton("Inventory Down Button", factory, "down/", 115f);

			okButton.OnMouseClick(Hide, _game);

			_invWindow = factory.Inventory.GetInventoryWindow("Inventory Window", 124f, 88f, 40f, 22f, 7f, 30f);
			_invWindow.TreeNode.SetParent(_panel.TreeNode);

			upButton.OnMouseClick(_invWindow.ScrollUp, _game);
			downButton.OnMouseClick(_invWindow.ScrollDown, _game);
		}

		public void Show()
		{
			_lastMode = _scheme.CurrentMode;
			_scheme.CurrentMode = MouseCursors.POINT_MODE;
			_scheme.RotatingEnabled = false;
			_panel.Visible = true;
		}

		public void Hide()
		{
			_scheme.RotatingEnabled = true;
			if (_lastMode != null && _game.State.Player.Character.Inventory.ActiveItem == null) _scheme.CurrentMode = _lastMode;
			_panel.Visible = false;
		}

		private void onSaveGameLoaded()
		{
			_panel = _game.Find<IPanel>(_panel.ID);
			_invWindow = _game.Find<IInventoryWindow>(_invWindow.ID);
		}

		private IButton loadButton(string id, IGameFactory factory, string folder, float x, string mode = null)
		{
			folder = _baseFolder + folder;
			IButton button = factory.UI.GetButton(id, folder + "normal.bmp", folder + "hovered.bmp", folder + "pushed.bmp", x, 4f);
			button.TreeNode.SetParent(_panel.TreeNode);
			if (mode != null)
			{
				button.OnMouseClick(() =>
				{
					_scheme.CurrentMode = mode;
					_game.State.Player.Character.Inventory.ActiveItem = null;
				}, _game);
			}
			return button;
		}
	}
}

