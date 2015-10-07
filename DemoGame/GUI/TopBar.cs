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

		public TopBar(RotatingCursorScheme scheme, InventoryPanel invPanel, OptionsPanel optionsPanel)
		{
			_scheme = scheme;
			_invPanel = invPanel;
			_optionsPanel = optionsPanel;
		}

		public void Load(IGameFactory factory)
		{
			_panel = factory.GetPanel("../../Assets/Gui/DialogBox/toolbar.bmp", 0f, 180f);

			loadButton(factory, "walk/", 0f, RotatingCursorScheme.WALK_MODE);
			loadButton(factory, "hand/", 20f, RotatingCursorScheme.INTERACT_MODE);
			loadButton(factory, "eye/", 40f, RotatingCursorScheme.LOOK_MODE);
			loadButton(factory, "talk/", 60f, MouseCursors.TALK_MODE);
			IButton invButton = loadButton(factory, "inventory/", 80f);
			loadButton(factory, "activeInventory/", 100f);
			loadButton(factory, "help/", 280f);
			IButton optionsButton = loadButton(factory, "settings/", 300f);

			invButton.Events.MouseClicked.Subscribe((sender, e) => _invPanel.Show());
			optionsButton.Events.MouseClicked.Subscribe((sender, e) => _optionsPanel.Show());
		}

		private IButton loadButton(IGameFactory factory, string folder, float x, string mode = null)
		{
			folder = _baseFolder + folder;
			IButton button = factory.GetButton(folder + "normal.bmp", folder + "hovered.bmp", folder + "pushed.bmp", x, 0f);
			button.TreeNode.SetParent(_panel.TreeNode);
			if (mode != null)
			{
				button.Events.MouseClicked.Subscribe((sender, e) => _scheme.CurrentMode = mode);
			}
			return button;
		}
	}
}

