using AGS.API;
using AGS.Engine;

namespace AGS.Editor
{
    public class GameToolbar
    {
        private readonly EditorShouldBlockEngineInput _blocker;
        private ILabel _fpsLabel, _mousePosLabel, _hotspotLabel;
        private IButton _playPauseButton;

        public GameToolbar(EditorShouldBlockEngineInput blocker) => _blocker = blocker;

        public void Init(IGameFactory factory)
        {
            var resolution = new Size(1200, 800);

            var moveCursor = factory.UI.GetLabel("MoveCursor", "", 25f, 25f, 0f, 0f, config: AGSTextConfig.ScaleConfig(FontIcons.IconConfig, 5f), addToUi: false);
            moveCursor.Text = FontIcons.Move;

            var toolbarHeight = resolution.Height / 20f;
            var toolbar = factory.UI.GetPanel("GameToolbar", resolution.Width / 2f, toolbarHeight, resolution.Width / 2f, 20f);
            toolbar.Pivot = new PointF(0.5f, 0f);
            toolbar.Tint = GameViewColors.SubPanel;
            toolbar.RenderLayer = new AGSRenderLayer(-99999, independentResolution: resolution);
            toolbar.ClickThrough = false;
            toolbar.Border = AGSBorders.SolidColor(GameViewColors.Border, 3f, true);
            toolbar.AddComponent<IDraggableComponent>();
            toolbar.AddComponent<IHasCursorComponent>().SpecialCursor = moveCursor;

            var idle = new ButtonAnimation(null, FontIcons.ButtonConfig, GameViewColors.Button);
            var hover = new ButtonAnimation(null, AGSTextConfig.ChangeColor(FontIcons.ButtonConfig, Colors.Yellow, Colors.White, 0f), GameViewColors.HoveredButton);
            var pushed = new ButtonAnimation(null, FontIcons.ButtonConfig, GameViewColors.PushedButton);
            const float buttonWidth = 50f;
            float buttonHeight = toolbar.Height * 3 / 4f;
            float buttonY = toolbar.Height / 2f;
            float buttonX = toolbar.Width / 2f;
            _playPauseButton = factory.UI.GetButton("PlayPauseGameButton", idle, hover, pushed, buttonX, buttonY, toolbar, width: buttonWidth, height: buttonHeight);
            _playPauseButton.Text = FontIcons.Pause;
            _playPauseButton.Pivot = new PointF(0.5f, 0.5f);
            _playPauseButton.RenderLayer = toolbar.RenderLayer;
            _playPauseButton.MouseClicked.Subscribe(onPlayPauseClicked);

            _fpsLabel = factory.UI.GetLabel("FPS Label", "", 30f, 25f, 0f, _playPauseButton.Y, toolbar, config: new AGSTextConfig(autoFit: AutoFit.LabelShouldFitText));
            _fpsLabel.Pivot = new PointF(0f, 0.5f);
            _fpsLabel.TextBackgroundVisible = false;
            _fpsLabel.RenderLayer = toolbar.RenderLayer;
            _fpsLabel.Enabled = true;
            _fpsLabel.MouseEnter.Subscribe(_ => _fpsLabel.Tint = Colors.Indigo);
            _fpsLabel.MouseLeave.Subscribe(_ => _fpsLabel.Tint = Colors.IndianRed.WithAlpha(125));


            _mousePosLabel = factory.UI.GetLabel("Mouse Position Label", "", 1f, 1f, 120f, _playPauseButton.Y, toolbar, config: new AGSTextConfig(autoFit: AutoFit.LabelShouldFitText));
            _mousePosLabel.TextBackgroundVisible = false;
            _mousePosLabel.Pivot = new PointF(0f, 0.5f);
            _mousePosLabel.RenderLayer = toolbar.RenderLayer;

            _hotspotLabel = factory.UI.GetLabel("Debug Hotspot Label", "", 250f, _fpsLabel.Height, toolbar.Width, _playPauseButton.Y, toolbar, config: new AGSTextConfig(alignment: Alignment.TopRight,
                                                                                                                                                     autoFit: AutoFit.TextShouldFitLabel));
            _hotspotLabel.TextBackgroundVisible = false;
            _hotspotLabel.Pivot = new PointF(1f, 0.5f);
            _hotspotLabel.RenderLayer = toolbar.RenderLayer;
        }

        private void onPlayPauseClicked(MouseButtonEventArgs obj)
        {
            _playPauseButton.Text = _playPauseButton.Text == FontIcons.Pause ? FontIcons.Play : FontIcons.Pause;
            _blocker.BlockEngine = _playPauseButton.Text == FontIcons.Play;
        }

        public void SetGame(IGame game)
        {
            FPSCounter fps = new FPSCounter(game, _fpsLabel);
            fps.Start();

            MousePositionLabel mouseLabel = new MousePositionLabel(game, _mousePosLabel);
            mouseLabel.Start();

            HotspotLabel hotspot = new HotspotLabel(game, _hotspotLabel) { DebugMode = true };
            hotspot.Start();
        }
    }
}