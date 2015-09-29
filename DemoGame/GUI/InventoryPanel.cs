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

		public InventoryPanel(RotatingCursorScheme scheme)
		{
			_scheme = scheme;
		}

		public void Load(IGameFactory factory)
		{
			_panel = factory.GetPanel("../../Assets/Gui/DialogBox/inventory.bmp", 160f, 100f);
			_panel.Anchor = new AGSPoint (0.5f, 0.5f);
			_panel.Visible = false;

			loadButton(factory, "magnify/", 5f, RotatingCursorScheme.LOOK_MODE);
			loadButton(factory, "upLeft/", 27f, MouseCursors.POINT_MODE);
			IButton okButton = loadButton(factory, "ok/", 49f);
			loadButton(factory, "up/", 93f);
			loadButton(factory, "down/", 115f);

			okButton.Events.MouseClicked.Subscribe((sender, e) => Hide());
		}

		public void Show()
		{
			_lastMode = _scheme.CurrentMode;
			_scheme.CurrentMode = MouseCursors.POINT_MODE;
			_panel.Visible = true;
		}

		public void Hide()
		{
			if (_lastMode != null) _scheme.CurrentMode = _lastMode;
			_panel.Visible = false;
		}

		private IButton loadButton(IGameFactory factory, string folder, float x, string mode = null)
		{
			folder = _baseFolder + folder;
			IButton button = factory.GetButton(folder + "normal.bmp", folder + "hovered.bmp", folder + "pushed.bmp", x, 4f);
			button.TreeNode.SetParent(_panel.TreeNode);
			if (mode != null)
			{
				button.Events.MouseClicked.Subscribe((sender, e) => _scheme.CurrentMode = mode);
			}
			return button;
		}
	}
}

