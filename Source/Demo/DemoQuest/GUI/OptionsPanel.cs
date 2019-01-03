using System;
using AGS.API;
using AGS.Engine;
using System.Threading.Tasks;

namespace DemoGame
{
    public class OptionsPanel
    {
        private const string _sliderFolder = "Gui/Sliders/";
        private const string _panelId = "Options Panel";
        private const string _buttonsPanelId = "Options Buttons Panel";
        private IPanel _panel, _buttonsPanel;
        private IGame _game;

        private static IFontFactory _fonts = AGSGame.Game.Factory.Fonts;
        ITextConfig _textConfig = _fonts.GetTextConfig(font: _fonts.LoadFont(null, 10f),
            brush: AGSGame.Device.BrushLoader.LoadSolidBrush(Colors.DarkOliveGreen),
            alignment: Alignment.MiddleCenter, autoFit: AutoFit.TextShouldFitLabel,
            outlineBrush: AGSGame.Device.BrushLoader.LoadSolidBrush(Colors.LightGreen), outlineWidth: 1f);

        ITextConfig _buttonTextConfig = _fonts.GetTextConfig(font: _fonts.LoadFont(null, 7f, FontStyle.Bold),
            brush: AGSGame.Device.BrushLoader.LoadSolidBrush(Colors.LightGreen),
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
            _panel = await factory.UI.GetPanelAsync(_panelId, "Gui/DialogBox/options.bmp", 160f, 100f);
            _panel.Pivot = (0.5f, 0.5f);
            _panel.Visible = false;
            _panel.AddComponent<IModalWindowComponent>();

            AGSLoadImageConfig loadConfig = new AGSLoadImageConfig(new Point(0, 0));

            ISlider volumeSlider = await factory.UI.GetSliderAsync("Volume Slider", _sliderFolder + "slider.bmp", 
               _sliderFolder + "handle.bmp", 0.5f, 0f, 1f, _panel, loadConfig: loadConfig);
            volumeSlider.Position = (120f, 10f);
            volumeSlider.HandleGraphics.Pivot = (0.5f, 0.5f);
            volumeSlider.OnValueChanged(onVolumeChanged, _game);

            ILabel volumeLabel = factory.UI.GetLabel("Volume Label", "Volume", 50f, 30f, 120f, 85f, _panel, _textConfig);
            volumeLabel.Pivot = (0.5f, 0f);

            ISlider speedSlider = await factory.UI.GetSliderAsync("Speed Slider", _sliderFolder + "slider.bmp", 
                _sliderFolder + "handle.bmp", 100f, 1f, 200f, _panel, loadConfig: loadConfig);
            speedSlider.Position = (180f, 10f);
            speedSlider.HandleGraphics.Pivot = (0.5f, 0.5f);
            speedSlider.OnValueChanged(onSpeedChanged, _game);

            ILabel speedLabel = factory.UI.GetLabel("Speed Label", "Speed", 50f, 30f, 180f, 85f, _panel, _textConfig);
            speedLabel.Pivot = (0.5f, 0f);

            _game.Events.OnSavedGameLoad.Subscribe(findPanel);

#if __IOS__
            const int top = 85;
            const int step = -25;
#else
            const int top = 95;
            const int step = -20;
#endif
            _buttonsPanel = factory.UI.GetPanel(_buttonsPanelId, (IImage)null, 15f, top, _panel);
            _buttonsPanel.AddComponent<IBoundingBoxWithChildrenComponent>();
            var layout = _buttonsPanel.AddComponent<IStackLayoutComponent>();
            layout.RelativeSpacing = 0f;
            layout.AbsoluteSpacing = step;
            layout.StartLayout();
            await loadButton("Resume", hide);
            await loadButton("Restart", restart);
            await loadButton("Load", load);
            await loadButton("Save", save);
#if !__IOS__ //IOS does not allow for a quit button in its guidelines
            await loadButton("Quit", quit);
#endif
        }

		private void findPanel()
		{
			_panel = _game.Find<IPanel>(_panelId);
            _buttonsPanel = _game.Find<IPanel>(_buttonsPanelId);
		}

		private async Task loadButton(string text, Action onClick)
		{
			const string folder = "Gui/Buttons/buttonSmall/";
			string buttonId = $"{text} Button";
			IButton button = await _game.Factory.UI.GetButtonAsync(buttonId, folder + "normal.bmp", folder + "hovered.bmp",
               folder + "pushed.bmp", 15f, 0f, _buttonsPanel, text, _buttonTextConfig);
			button.OnMouseClick(onClick, _game);
		}

		private void onSpeedChanged(float speed)
		{
			_game.State.Speed = (int)speed;
		}

		private void onVolumeChanged(float volume)
		{
			_game.Audio.Settings.MasterVolume = volume;
		}

		public void Show()
		{
			_lastMode = _scheme.CurrentMode;
			_scheme.CurrentMode = MouseCursors.POINT_MODE;
			_scheme.RotatingEnabled = false;
			_panel.Visible = true;
            _panel.GetComponent<IModalWindowComponent>().GrabFocus();
		}

        private void hide()
		{
			_scheme.RotatingEnabled = true;
			if (_game.State.Player.Inventory.ActiveItem == null)
				_scheme.CurrentMode = _lastMode;
			else _scheme.SetInventoryCursor();
			_panel.Visible = false;
            _panel.GetComponent<IModalWindowComponent>().LoseFocus();
		}

		private async void save()
		{
            _game.Settings.Defaults.Skin = new AGSBlueSkin(_game.Factory.Graphics).CreateSkin();
            string file = await AGSSelectFileDialog.SelectFile("Select file to save", FileSelection.FileOnly);
            _game.Settings.Defaults.Skin = null;
            if (file == null) return;
            _game.SaveLoad.Save(file);
			hide();
		}

		private async void load()
		{
            _game.Settings.Defaults.Skin = new AGSBlueSkin(_game.Factory.Graphics).CreateSkin();
            string file = await AGSSelectFileDialog.SelectFile("Select file to load", FileSelection.FileOnly);
            _game.Settings.Defaults.Skin = null;
            if (file == null) return;
            _game.SaveLoad.Load(file);
			hide();
		}

		private void restart()
		{
			_game.SaveLoad.Restart();
		}

		private async void quit()
		{
			if (!await AGSMessageBox.YesNoAsync("Are you sure you want to quit?")) return;
			_game.Quit();
		}
	}
}
