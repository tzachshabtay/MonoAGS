using System;
using AGS.API;
using AGS.Engine;
using System.Drawing;

namespace DemoGame
{
	public class OptionsPanel
	{
		private const string _sliderFolder = "../../Assets/Gui/Sliders/";
		private IPanel _panel;
		private IGame _game;

		AGSTextConfig _textConfig = new AGSTextConfig (font: new Font (SystemFonts.DefaultFont.FontFamily, 10f), brush: Brushes.DarkOliveGreen,
			outlineBrush: Brushes.LightGreen, outlineWidth: 1f);

		AGSTextConfig _buttonTextConfig = new AGSTextConfig (font: new Font (SystemFonts.DefaultFont.FontFamily, 7f, FontStyle.Bold), brush: Brushes.LightGreen,
			alignment: ContentAlignment.MiddleCenter);

		private string _lastMode;
		private readonly RotatingCursorScheme _scheme;

		public OptionsPanel(RotatingCursorScheme scheme)
		{
			_scheme = scheme;
		}

		public void Load(IGame game)
		{
			_game = game;
			IGameFactory factory = game.Factory;
			_panel = factory.UI.GetPanel("../../Assets/Gui/DialogBox/options.bmp", 160f, 100f);
			_panel.Anchor = new AGSPoint (0.5f, 0.5f);
			_panel.Visible = false;

			AGSLoadImageConfig loadConfig = new AGSLoadImageConfig { TransparentColorSamplePoint = new Point (0, 0) };

			ISlider volumeSlider = factory.UI.GetSlider(_sliderFolder + "slider.bmp", _sliderFolder + "handle.bmp", 50f, 0f, 100f, 
				loadConfig: loadConfig);
			volumeSlider.X = 120f;
			volumeSlider.Y = 10f;
			volumeSlider.HandleGraphics.Anchor = new AGSPoint (0.5f, 0.5f);
			volumeSlider.TreeNode.SetParent(_panel.TreeNode);

			ILabel volumeLabel = factory.UI.GetLabel("Volume", 50f, 30f, 120f, 85f, _textConfig); 
			volumeLabel.Anchor = new AGSPoint (0.5f, 0f);
			volumeLabel.TreeNode.SetParent(_panel.TreeNode);

			ISlider speedSlider = factory.UI.GetSlider(_sliderFolder + "slider.bmp", _sliderFolder + "handle.bmp", 50f, 0f, 100f, 
				loadConfig: loadConfig);
			speedSlider.X = 180f;
			speedSlider.Y = 10f;
			speedSlider.HandleGraphics.Anchor = new AGSPoint (0.5f, 0.5f);
			speedSlider.TreeNode.SetParent(_panel.TreeNode);

			ILabel speedLabel = factory.UI.GetLabel("Speed", 50f, 30f, 180f, 85f, _textConfig); 
			speedLabel.Anchor = new AGSPoint (0.5f, 0f);
			speedLabel.TreeNode.SetParent(_panel.TreeNode);

			IButton resumeButton = loadButton("Resume", 95);
			resumeButton.Events.MouseClicked.Subscribe((sender, args) => Hide());

			IButton restartButton = loadButton("Restart", 75);
			restartButton.Events.MouseClicked.Subscribe((sender, args) => Hide());

			IButton restoreButton = loadButton("Load", 55);
			restoreButton.Events.MouseClicked.Subscribe((sender, args) => load());

			IButton saveButton = loadButton("Save", 35);
			saveButton.Events.MouseClicked.Subscribe((sender, args) => save());

			IButton quitButton = loadButton("Quit", 15);
			quitButton.Events.MouseClicked.Subscribe((sender, args) => Hide());
		}

		private IButton loadButton(string text, float y)
		{
			const string folder = "../../Assets/Gui/Buttons/buttonSmall/";
			IButton button = _game.Factory.UI.GetButton(folder + "normal.bmp", folder + "hovered.bmp", 
				folder + "pushed.bmp", 15f, y, text, _buttonTextConfig);
			button.TreeNode.SetParent(_panel.TreeNode);
			return button;
		}

		private void save()
		{
			_game.SaveLoad.Save("save.bin");
			Hide();
		}

		private void load()
		{
			_game.SaveLoad.Load("save.bin");
			Hide();
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
			if (_game.State.Player.Character.Inventory.ActiveItem == null)
				_scheme.CurrentMode = _lastMode;
			else _scheme.SetInventoryCursor();
			_panel.Visible = false;
		}
	}
}

