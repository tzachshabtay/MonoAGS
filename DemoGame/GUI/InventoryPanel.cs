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

		public InventoryPanel(RotatingCursorScheme scheme)
		{
			_scheme = scheme;
		}

		public void Load(IGame game)
		{
			_game = game;
			IGameFactory factory = game.Factory;

			_panel = factory.UI.GetPanel("../../Assets/Gui/DialogBox/inventory.bmp", 160f, 100f);
			_panel.Anchor = new AGSPoint (0.5f, 0.5f);
			_panel.Visible = false;

			loadButton(factory, "magnify/", 5f, RotatingCursorScheme.LOOK_MODE);
			loadButton(factory, "upLeft/", 27f, MouseCursors.POINT_MODE);
			IButton okButton = loadButton(factory, "ok/", 49f);
			IButton upButton = loadButton(factory, "up/", 93f);
			IButton downButton = loadButton(factory, "down/", 115f);

			okButton.Events.MouseClicked.Subscribe((sender, e) => Hide());

			var invWindow = factory.Inventory.GetInventoryWindow(124f, 88f, 40f, 22f, 7f, 30f);
			invWindow.TreeNode.SetParent(_panel.TreeNode);

			upButton.Events.MouseClicked.Subscribe((sender, e) => invWindow.ScrollUp());
			downButton.Events.MouseClicked.Subscribe((sender, e) => invWindow.ScrollDown());	
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

		private IButton loadButton(IGameFactory factory, string folder, float x, string mode = null)
		{
			folder = _baseFolder + folder;
			IButton button = factory.UI.GetButton(folder + "normal.bmp", folder + "hovered.bmp", folder + "pushed.bmp", x, 4f);
			button.TreeNode.SetParent(_panel.TreeNode);
			if (mode != null)
			{
				button.Events.MouseClicked.Subscribe((sender, e) => 
				{
					_scheme.CurrentMode = mode;
					_game.State.Player.Character.Inventory.ActiveItem = null;
				});
			}
			return button;
		}
	}
}

