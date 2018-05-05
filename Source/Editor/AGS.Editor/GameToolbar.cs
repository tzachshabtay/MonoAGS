using System;
using System.ComponentModel;
using AGS.API;
using AGS.Engine;

namespace AGS.Editor
{
    public class GameToolbar
    {
        private readonly EditorShouldBlockEngineInput _blocker;
        private readonly IInput _editorInput;
        private IWindowInfo _windowInfo;
        private IInput _gameInput;
        private ILabel _fpsLabel, _mousePosLabel, _hotspotLabel;
        private IButton _playPauseButton;
        private IObject _lastGameCursor;
        private IPanel _toolbar;
        private Size _resolution = new Size(1200, 800);

        private ILabel _pointer;

        public GameToolbar(EditorShouldBlockEngineInput blocker, IInput editorInput)
        {
            _editorInput = editorInput;
            _blocker = blocker;
        }

        public void Init(IGameFactory factory)
        {
            _pointer = factory.UI.GetLabel("PointerCursor", "", 25f, 25f, 0f, 0f, config: FontIcons.IconConfig, addToUi: false);
            _pointer.Text = FontIcons.Pointer;
            _pointer.Pivot = new PointF(0.29f, 0.83f);

            var toolbarHeight = _resolution.Height / 20f;
            _toolbar = factory.UI.GetPanel("GameToolbar", _resolution.Width / 2f, toolbarHeight, _resolution.Width / 2f, _resolution.Height - toolbarHeight);
            _toolbar.Pivot = new PointF(0.5f, 0f);
            _toolbar.Tint = GameViewColors.SubPanel;
            _toolbar.RenderLayer = new AGSRenderLayer(-99999, independentResolution: _resolution);
            _toolbar.ClickThrough = false;
            _toolbar.Border = AGSBorders.SolidColor(GameViewColors.Border, 3f, true);

            var idle = new ButtonAnimation(null, FontIcons.ButtonConfig, GameViewColors.Button);
            var hover = new ButtonAnimation(null, AGSTextConfig.ChangeColor(FontIcons.ButtonConfig, Colors.Yellow, Colors.White, 0f), GameViewColors.HoveredButton);
            var pushed = new ButtonAnimation(null, FontIcons.ButtonConfig, GameViewColors.PushedButton);
            const float buttonWidth = 50f;
            float buttonHeight = _toolbar.Height * 3 / 4f;
            float buttonY = _toolbar.Height / 2f;
            float buttonX = _toolbar.Width / 2f;
            _playPauseButton = factory.UI.GetButton("PlayPauseGameButton", idle, hover, pushed, buttonX, buttonY, _toolbar, width: buttonWidth, height: buttonHeight);
            _playPauseButton.Text = FontIcons.Pause;
            _playPauseButton.Pivot = new PointF(0.5f, 0.5f);
            _playPauseButton.RenderLayer = _toolbar.RenderLayer;
            _playPauseButton.MouseClicked.Subscribe(onPlayPauseClicked);

            _fpsLabel = factory.UI.GetLabel("FPS Label", "", 30f, 25f, 0f, _playPauseButton.Y, _toolbar, config: new AGSTextConfig(autoFit: AutoFit.LabelShouldFitText));
            _fpsLabel.Pivot = new PointF(0f, 0.5f);
            _fpsLabel.TextBackgroundVisible = false;
            _fpsLabel.RenderLayer = _toolbar.RenderLayer;
            _fpsLabel.Enabled = true;
            _fpsLabel.MouseEnter.Subscribe(_ => _fpsLabel.Tint = Colors.Indigo);
            _fpsLabel.MouseLeave.Subscribe(_ => _fpsLabel.Tint = Colors.IndianRed.WithAlpha(125));

            _mousePosLabel = factory.UI.GetLabel("Mouse Position Label", "", 1f, 1f, 120f, _playPauseButton.Y, _toolbar, config: new AGSTextConfig(autoFit: AutoFit.LabelShouldFitText));
            _mousePosLabel.TextBackgroundVisible = false;
            _mousePosLabel.Pivot = new PointF(0f, 0.5f);
            _mousePosLabel.RenderLayer = _toolbar.RenderLayer;

            _hotspotLabel = factory.UI.GetLabel("Debug Hotspot Label", "", 250f, _fpsLabel.Height, _toolbar.Width, _playPauseButton.Y, _toolbar, config: new AGSTextConfig(alignment: Alignment.TopRight,
                                                                                                                                                     autoFit: AutoFit.TextShouldFitLabel));
            _hotspotLabel.TextBackgroundVisible = false;
            _hotspotLabel.Pivot = new PointF(1f, 0.5f);
            _hotspotLabel.RenderLayer = _toolbar.RenderLayer;
        }

        private void setPosition()
        {
            float x = MathUtils.Lerp(0f, 0f, _windowInfo.AppWindowWidth, _resolution.Width, _windowInfo.ScreenViewport.X);
            float width = MathUtils.Lerp(0f, 0f, _windowInfo.AppWindowWidth, _resolution.Width, _windowInfo.ScreenViewport.Width);
            _toolbar.X = x + width / 2f;
        }

        private void onGameWindowPropertyChanged(object sender, PropertyChangedEventArgs args)
        {
            if (args.PropertyName != nameof(IWindowInfo.ScreenViewport)) return;
            setPosition();
        }

        private void onPlayPauseClicked(MouseButtonEventArgs obj)
        {
            _playPauseButton.Text = _playPauseButton.Text == FontIcons.Pause ? FontIcons.Play : FontIcons.Pause;
            bool isPaused = _playPauseButton.Text == FontIcons.Play;
            _blocker.BlockEngine = isPaused;
            _editorInput.Cursor = isPaused ? _pointer : null;
            if (isPaused) _lastGameCursor = _gameInput.Cursor;
            _gameInput.Cursor = isPaused ? null : _lastGameCursor;
        }

        public void SetGame(IGame game, IWindowInfo gameWindow)
        {
            _windowInfo = gameWindow;
            gameWindow.PropertyChanged += onGameWindowPropertyChanged;
            setPosition();

            _gameInput = game.Input;

            FPSCounter fps = new FPSCounter(game, _fpsLabel);
            fps.Start();

            MousePositionLabel mouseLabel = new MousePositionLabel(game, _mousePosLabel);
            mouseLabel.Start();

            HotspotLabel hotspot = new HotspotLabel(game, _hotspotLabel) { DebugMode = true };
            hotspot.Start();
        }
    }
}