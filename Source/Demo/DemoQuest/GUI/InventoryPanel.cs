using System.Threading.Tasks;
using AGS.API;
using AGS.Engine;

namespace DemoGame
{
	public class InventoryPanel
	{
		private const string _baseFolder = "Gui/Buttons/";
		private IPanel _panel;
		private readonly RotatingCursorScheme _scheme;
		private string _lastMode;
		private IGame _game;
		private IInventoryWindow _invWindow;

		public InventoryPanel(RotatingCursorScheme scheme)
		{
			_scheme = scheme;
		}

		public async Task LoadAsync(IGame game)
		{
			_game = game;
            _game.Events.OnSavedGameLoad.Subscribe(onSaveGameLoaded);
			IGameFactory factory = game.Factory;

			_panel = await factory.UI.GetPanelAsync("Inventory Panel", "Gui/DialogBox/inventory.bmp", 160f, 100f);
			_panel.Pivot = new PointF (0.5f, 0.5f);
			_panel.Visible = false;

			await loadButton("Inventory Look Button", factory, "magnify/", 5f, RotatingCursorScheme.LOOK_MODE);
			await loadButton("Invntory Point Button", factory, "upLeft/", 27f, MouseCursors.POINT_MODE);
			IButton okButton = await loadButton("Inventory Ok Button", factory, "ok/", 49f);
			IButton upButton = await loadButton("Inventory Up Button", factory, "up/", 93f);
			IButton downButton = await loadButton("Inventory Down Button", factory, "down/", 115f);

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
			if (_lastMode != null && _game.State.Player.Inventory.ActiveItem == null) _scheme.CurrentMode = _lastMode;
			_panel.Visible = false;
		}

		private void onSaveGameLoaded()
		{
			_panel = _game.Find<IPanel>(_panel.ID);
			_invWindow = _game.Find<IInventoryWindow>(_invWindow.ID);
		}

		private async Task<IButton> loadButton(string id, IGameFactory factory, string folder, float x, string mode = null)
		{
			folder = _baseFolder + folder;
			IButton button = await factory.UI.GetButtonAsync(id, folder + "normal.bmp", folder + "hovered.bmp", 
                                                             folder + "pushed.bmp", x, 4f, _panel);
			if (mode != null)
			{
				button.OnMouseClick(() =>
				{
					_scheme.CurrentMode = mode;
					_game.State.Player.Inventory.ActiveItem = null;
				}, _game);
			}
			return button;
		}
	}
}

