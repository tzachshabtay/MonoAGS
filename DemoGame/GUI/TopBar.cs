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

		public TopBar(RotatingCursorScheme scheme)
		{
			_scheme = scheme;
		}

		public void Load(IGameFactory factory)
		{
			_panel = factory.GetPanel("../../Assets/Gui/DialogBox/toolbar.bmp", 0f, 180f);

			loadButton(factory, "walk/", 0f, RotatingCursorScheme.WALK_MODE);
			loadButton(factory, "eye/", 20f, RotatingCursorScheme.LOOK_MODE);
			loadButton(factory, "hand/", 40f, RotatingCursorScheme.INTERACT_MODE);
			loadButton(factory, "talk/", 60f, MouseCursors.TALK_MODE);
			loadButton(factory, "inventory/", 80f);
			loadButton(factory, "activeInventory/", 100f);
			loadButton(factory, "settings/", 280f);
			loadButton(factory, "help/", 300f);
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

