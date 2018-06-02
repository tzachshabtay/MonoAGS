using System;
using System.ComponentModel;
using AGS.API;
using AGS.Engine;
using Autofac;

namespace AGS.Editor
{
    public class GameToolbar
    {
        private readonly EditorShouldBlockEngineInput _blocker;
        private readonly IInput _editorInput;
        private readonly IFont _font;
        private IWindowInfo _windowInfo;
        private IGame _game;
        private ILabel _fpsLabel, _mousePosLabel, _hotspotLabel;
        private IButton _playPauseButton;
        private IObject _lastGameCursor;
        private IPanel _toolbar;
        private Size _resolution = new Size(1200, 800);
        private GameDebugTree _tree;

        private ILabel _pointer;

        public GameToolbar(EditorShouldBlockEngineInput blocker, IInput editorInput, IGameSettings settings)
        {
            _editorInput = editorInput;
            _editorInput.MouseMove.Subscribe(onMouseMove);
            _blocker = blocker;
            _font = settings.Defaults.TextFont;
        }

        public bool IsPaused => _playPauseButton.Text == FontIcons.Play;

        public void Init(IGameFactory factory, AGSEditor editor)
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
            _toolbar.Border = AGSBorders.SolidColor(editor.EditorResolver.Container.Resolve<IGLUtils>(), editor.Editor.Settings,  GameViewColors.Border, 3f, true);

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

            _fpsLabel = factory.UI.GetLabel("FPS Label (Editor)", "", 30f, 25f, 0f, _playPauseButton.Y, _toolbar, config: new AGSTextConfig(autoFit: AutoFit.LabelShouldFitText, font: _font));
            _fpsLabel.Pivot = new PointF(0f, 0.5f);
            _fpsLabel.TextBackgroundVisible = false;
            _fpsLabel.RenderLayer = _toolbar.RenderLayer;
            _fpsLabel.Enabled = true;
            _fpsLabel.MouseEnter.Subscribe(_ => _fpsLabel.Tint = Colors.Indigo);
            _fpsLabel.MouseLeave.Subscribe(_ => _fpsLabel.Tint = Colors.IndianRed.WithAlpha(125));

            _mousePosLabel = factory.UI.GetLabel("Mouse Position Label (Editor)", "", 1f, 1f, 120f, _playPauseButton.Y, _toolbar, config: new AGSTextConfig(autoFit: AutoFit.LabelShouldFitText, font: _font));
            _mousePosLabel.TextBackgroundVisible = false;
            _mousePosLabel.Pivot = new PointF(0f, 0.5f);
            _mousePosLabel.RenderLayer = _toolbar.RenderLayer;

            _hotspotLabel = factory.UI.GetLabel("Debug Hotspot Label (Editor)", "", 250f, _fpsLabel.Height, _toolbar.Width, _playPauseButton.Y, _toolbar, config: new AGSTextConfig(alignment: Alignment.TopRight,
                                                                                                                                                                           autoFit: AutoFit.TextShouldFitLabel, font: _font));
            _hotspotLabel.TextBackgroundVisible = false;
            _hotspotLabel.Pivot = new PointF(1f, 0.5f);
            _hotspotLabel.RenderLayer = _toolbar.RenderLayer;
        }

        private void setPosition()
        {
            var viewport = _game.State.Viewport;
            float x = MathUtils.Lerp(0f, 0f, _windowInfo.AppWindowWidth, _resolution.Width, viewport.ScreenArea.X);
            float width = MathUtils.Lerp(0f, 0f, _windowInfo.AppWindowWidth, _resolution.Width, viewport.ScreenArea.Width);
            _toolbar.X = x + width / 2f;
        }

        private void onViewportPropertyChanged(object sender, PropertyChangedEventArgs args)
        {
            if (args.PropertyName != nameof(IViewport.ScreenArea)) return;
            setPosition();
        }

        private void onPlayPauseClicked(MouseButtonEventArgs obj)
        {
            _playPauseButton.Text = _playPauseButton.Text == FontIcons.Pause ? FontIcons.Play : FontIcons.Pause;
            _blocker.BlockEngine = IsPaused;
            updateEditorCursor(_editorInput.MousePosition);
            if (IsPaused) _lastGameCursor = _game.Input.Cursor;
            else _tree?.Unselect();
            _game.Input.Cursor = IsPaused ? null : _lastGameCursor;
        }

        public void SetGame(IGame game, IWindowInfo gameWindow, GameDebugTree tree)
        {
            _tree = tree;
            _windowInfo = gameWindow;
            _game = game;

            game.State.Viewport.PropertyChanged += onViewportPropertyChanged;
            setPosition();

            FPSCounter fps = new FPSCounter(game, _fpsLabel);
            fps.Start();

            MousePositionLabel mouseLabel = new MousePositionLabel(game, _mousePosLabel);
            mouseLabel.Start();

            HotspotLabel hotspot = new HotspotLabel(game, _hotspotLabel) { DebugMode = true };
            hotspot.Start();
        }

        private void onMouseMove(MousePositionEventArgs args)
        {
            updateEditorCursor(args.MousePosition);
        }

        private void updateEditorCursor(MousePosition mousePosition)
        {
            if (IsPaused)
            {
                if (_editorInput.Cursor != null) return;
                _editorInput.Cursor = _pointer;
                return;
            }

            var viewport = _game.State.Viewport;
            var projectBottom = _windowInfo.AppWindowHeight - viewport.ScreenArea.Y - viewport.ProjectionBox.Y * viewport.ScreenArea.Height;
            var projectTop = projectBottom - viewport.ProjectionBox.Height * viewport.ScreenArea.Height;

            if (mousePosition.XWindow < viewport.ScreenArea.X ||
                mousePosition.XWindow > viewport.ScreenArea.X + viewport.ScreenArea.Width ||
                mousePosition.YWindow < projectTop ||
                mousePosition.YWindow > projectBottom)
            {
                _editorInput.Cursor = _pointer;
            }
            else
            {
                _editorInput.Cursor = null;
            }
        }
    }
}
