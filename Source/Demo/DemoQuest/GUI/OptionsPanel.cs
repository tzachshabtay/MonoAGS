using System;
using AGS.API;
using AGS.Engine;
using System.Linq;
using System.Threading.Tasks;

namespace DemoGame
{
	public class OptionsPanel
	{
		private const string _sliderFolder = "../../Assets/Gui/Sliders/";
		private const string _panelId = "Options Panel";
		private IPanel _panel;
		private IGame _game;

		AGSTextConfig _textConfig = new AGSTextConfig (font: Hooks.FontLoader.LoadFont(null, 10f),
			brush: Hooks.BrushLoader.LoadSolidBrush(Colors.DarkOliveGreen),
			outlineBrush: Hooks.BrushLoader.LoadSolidBrush(Colors.LightGreen), outlineWidth: 1f);

		AGSTextConfig _buttonTextConfig = new AGSTextConfig (font: Hooks.FontLoader.LoadFont(null, 7f, FontStyle.Bold), 
			brush: Hooks.BrushLoader.LoadSolidBrush(Colors.LightGreen),
			alignment: Alignment.MiddleCenter, autoFit: AutoFit.TextShouldFitLabel, paddingLeft: 0f);

		private string _lastMode;
		private readonly RotatingCursorScheme _scheme;

		public OptionsPanel(RotatingCursorScheme scheme)
		{
			_scheme = scheme;
		}

		public async Task LoadAsync(IGame game)
		{
			_game = game;
			IGameFactory factory = game.Factory;
			_panel = await factory.UI.GetPanelAsync(_panelId, "../../Assets/Gui/DialogBox/options.bmp", 160f, 100f);
			_panel.Anchor = new AGS.API.PointF (0.5f, 0.5f);
			_panel.Visible = false;

			AGSLoadImageConfig loadConfig = new AGSLoadImageConfig { TransparentColorSamplePoint = new AGS.API.Point (0, 0) };

			ISlider volumeSlider = await factory.UI.GetSliderAsync("Volume Slider", _sliderFolder + "slider.bmp", _sliderFolder + "handle.bmp", 0.5f, 0f, 1f, 
				loadConfig: loadConfig);
			volumeSlider.X = 120f;
			volumeSlider.Y = 10f;
			volumeSlider.HandleGraphics.Anchor = new AGS.API.PointF (0.5f, 0.5f);
			volumeSlider.TreeNode.SetParent(_panel.TreeNode);
			volumeSlider.OnValueChanged(onVolumeChanged, _game);

			ILabel volumeLabel = factory.UI.GetLabel("Volume Label", "Volume", 50f, 30f, 120f, 85f, _textConfig); 
			volumeLabel.Anchor = new AGS.API.PointF (0.5f, 0f);
			volumeLabel.TreeNode.SetParent(_panel.TreeNode);

			ISlider speedSlider = await factory.UI.GetSliderAsync("Speed Slider", _sliderFolder + "slider.bmp", _sliderFolder + "handle.bmp", 100f, 1f, 200f, 
				loadConfig: loadConfig);
			speedSlider.X = 180f;
			speedSlider.Y = 10f;
			speedSlider.HandleGraphics.Anchor = new AGS.API.PointF (0.5f, 0.5f);
			speedSlider.TreeNode.SetParent(_panel.TreeNode);
			speedSlider.OnValueChanged(onSpeedChanged, _game);

			ILabel speedLabel = factory.UI.GetLabel("Speed Label", "Speed", 50f, 30f, 180f, 85f, _textConfig); 
			speedLabel.Anchor = new AGS.API.PointF (0.5f, 0f);
			speedLabel.TreeNode.SetParent(_panel.TreeNode);

			_game.Events.OnSavedGameLoad.Subscribe((sender, args) => findPanel());

            await loadButton("Resume", 95, Hide);
            await loadButton("Restart", 75, restart);
			await loadButton("Load", 55, load);
			await loadButton("Save", 35, save);
			await loadButton("Quit", 15, quit);
		}

		private void findPanel()
		{
			_panel = _game.Find<IPanel>(_panelId);
		}

		private async Task loadButton(string text, float y, Action onClick)
		{
			const string folder = "../../Assets/Gui/Buttons/buttonSmall/";
			string buttonId = string.Format("{0} Button", text);
			IButton button = await _game.Factory.UI.GetButtonAsync(buttonId, folder + "normal.bmp", folder + "hovered.bmp", 
				folder + "pushed.bmp", 15f, y, text, _buttonTextConfig);
			button.TreeNode.SetParent(_panel.TreeNode);
			button.OnMouseClick(onClick, _game);
		}

		private void onSpeedChanged(float speed)
		{
			_game.State.Speed = (int)speed;
		}

		private void onVolumeChanged(float volume)
		{
			_game.AudioSettings.MasterVolume = volume;
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

		private async void save()
		{
            AGSGameSettings.CurrentSkin = new AGSBlueSkin(_game.Factory.Graphics).CreateSkin();
            string file = await AGSSelectFileDialog.SelectFile("Select file to save", FileSelection.FileOnly);
            AGSGameSettings.CurrentSkin = null;
            if (file == null) return;
            _game.SaveLoad.Save(file);
			Hide();
		}

		private async void load()
		{
            AGSGameSettings.CurrentSkin = new AGSBlueSkin(_game.Factory.Graphics).CreateSkin();
            string file = await AGSSelectFileDialog.SelectFile("Select file to load", FileSelection.FileOnly);
            AGSGameSettings.CurrentSkin = null;
            if (file == null) return;
            _game.SaveLoad.Load(file);
			Hide();
		}

		private void restart()
		{
			_game.SaveLoad.Restart();
		}

		private void quit()
		{
			if (!AGSMessageBox.YesNo("Are you sure you want to quit?")) return;
			_game.Quit();
		}
	}
}

