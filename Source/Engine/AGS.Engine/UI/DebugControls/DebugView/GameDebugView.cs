using System.Threading.Tasks;
using AGS.API;

namespace AGS.Engine
{
    public class GameDebugView
    {
        private readonly IRenderLayer _layer;
        private readonly IGame _game;
        private readonly GameDebugTree _debugTree;
        private readonly GameDebugDisplayList _displayList;
        private readonly InspectorPanel _inspector;
        private const string _panelId = "Game Debug Tree Panel";
        private IPanel _panel;
        private ISplitPanelComponent _splitPanel;
        private IDebugTab _currentTab;
        private IButton _panesButton;

        public GameDebugView(IGame game)
        {
            _game = game;
            _layer = new AGSRenderLayer(AGSLayers.UI.Z - 1, independentResolution: new Size(1800, 1200));
            _inspector = new InspectorPanel(game, _layer);
            _debugTree = new GameDebugTree(game, _layer, _inspector);
            _displayList = new GameDebugDisplayList(game, _layer);
        }

        public bool Visible { get { return _panel.Visible; } }

        public void Load()
        {
            const float headerHeight = 50f;
            const float borderWidth = 3f;
            IGameFactory factory = _game.Factory;
            _panel = factory.UI.GetPanel(_panelId, _layer.IndependentResolution.Value.Width / 4f, _layer.IndependentResolution.Value.Height,
                                                     5f, _layer.IndependentResolution.Value.Height / 2f);
            _panel.Anchor = new PointF(0f, 0.5f);
            _panel.Visible = false;
            _panel.Tint = Colors.Black.WithAlpha(150);
            _panel.Border = AGSBorders.SolidColor(Colors.Green, borderWidth, hasRoundCorners: true);
            _panel.RenderLayer = _layer;
            _panel.ClickThrough = false;
            _game.State.FocusedUI.CannotLoseFocus.Add(_panelId);

            var headerLabel = factory.UI.GetLabel("GameDebugTreeLabel", "Game Debug", _panel.Width, headerHeight, 0f, _panel.Height - headerHeight,
                                      _panel, new AGSTextConfig(alignment: Alignment.MiddleCenter, autoFit: AutoFit.TextShouldFitLabel));
            headerLabel.Tint = Colors.Transparent;
            headerLabel.Border = _panel.Border;
            headerLabel.RenderLayer = _layer;

            var xButton = factory.UI.GetButton("GameDebugTreeCloseButton", (IAnimation)null, null, null, 0f, _panel.Height - headerHeight + 5f, _panel, "X",
                                               new AGSTextConfig(factory.Graphics.Brushes.LoadSolidBrush(Colors.Red),
                                                                 autoFit: AutoFit.TextShouldFitLabel, alignment: Alignment.MiddleCenter),
                                                                 width: 40f, height: 40f);
            xButton.Anchor = new PointF();
            xButton.RenderLayer = _layer;
            xButton.Tint = Colors.Transparent;
            xButton.MouseEnter.Subscribe(_ => xButton.TextConfig = AGSTextConfig.ChangeColor(xButton.TextConfig, Colors.Yellow, Colors.White, 0.3f));
            xButton.MouseLeave.Subscribe(_ => xButton.TextConfig = AGSTextConfig.ChangeColor(xButton.TextConfig, Colors.Red, Colors.Transparent, 0f));
            xButton.MouseClicked.Subscribe(_ => Hide());

            _panesButton = factory.UI.GetButton("GameDebugViewPanesButton", (IAnimation)null, null, null, _panel.Width, xButton.Y, _panel, "Display List",
                                                   new AGSTextConfig(autoFit: AutoFit.TextShouldFitLabel, alignment: Alignment.MiddleRight),
                                                   width: 120f, height: 40f);
            _panesButton.Anchor = new PointF(1f, 0f);
            _panesButton.RenderLayer = _layer;
            _panesButton.Tint = Colors.Black;
            _panesButton.MouseEnter.Subscribe(_ => _panesButton.TextConfig = AGSTextConfig.ChangeColor(xButton.TextConfig, Colors.Yellow, Colors.White, 0.3f));
            _panesButton.MouseLeave.Subscribe(_ => _panesButton.TextConfig = AGSTextConfig.ChangeColor(xButton.TextConfig, Colors.White, Colors.Transparent, 0f));
            _panesButton.MouseClicked.SubscribeToAsync(onPaneSwitch);

            var parentPanelHeight = _panel.Height - headerHeight;
            var parentPanel = factory.UI.GetPanel("GameDebugParentPanel", _panel.Width, parentPanelHeight, 0f, parentPanelHeight, _panel);
            parentPanel.Anchor = new PointF(0f, 1f);
            parentPanel.Tint = Colors.Transparent;
            parentPanel.RenderLayer = _layer;

            _debugTree.Load(parentPanel);
            _displayList.Load(parentPanel);
            _inspector.Load(parentPanel);
            _currentTab = _debugTree;
            _splitPanel = parentPanel.AddComponent<ISplitPanelComponent>();
            _splitPanel.TopPanel = _debugTree.Panel;
            _splitPanel.BottomPanel = _inspector.Panel;

            var horizSplit = _panel.AddComponent<ISplitPanelComponent>();
            horizSplit.IsHorizontal = true;
            horizSplit.TopPanel = _panel;

            _panel.GetComponent<IScaleComponent>().PropertyChanged += (_, args) => 
            {
                if (args.PropertyName != nameof(IScaleComponent.Width)) return;
                _panesButton.X = _panel.Width;
                headerLabel.LabelRenderSize = new SizeF(_panel.Width, headerLabel.LabelRenderSize.Height);
                parentPanel.BaseSize = new SizeF(_panel.Width, parentPanel.Height);
                _currentTab.Resize();
                _inspector.Resize();
            };
        }

        public Task Show()
        {
            _panel.Visible = true;
            _splitPanel.TopPanel = _currentTab.Panel;
            return _currentTab.Show();
        }

        public void Hide()
        {
            _panel.Visible = false;
            _currentTab.Hide();
        }

        private Task onPaneSwitch(MouseButtonEventArgs args)
        {
            _currentTab.Hide();
            _currentTab = (_currentTab == _debugTree) ? (IDebugTab)_displayList : _debugTree;
            _panesButton.Text = _currentTab == _debugTree ? "Display List" : "Scene Tree";
            _splitPanel.TopPanel = _currentTab.Panel;
            return _currentTab.Show();
        }
    }
}
