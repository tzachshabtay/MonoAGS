using AGS.API;
using AGS.Engine;

namespace DemoGame
{
    public class FeaturesTopWindow
    {
		private const string _panelId = "Features Panel";
		private IPanel _panel;
		private IGame _game;

		private string _lastMode;
		private readonly RotatingCursorScheme _scheme;
        private readonly IRenderLayer _layer;

		public FeaturesTopWindow(RotatingCursorScheme scheme)
		{
			_scheme = scheme;
            _layer = new AGSRenderLayer(AGSLayers.UI.Z, independentResolution: new Size(1200, 800));
		}

        public void Load(IGame game)
        {
            const float headerHeight = 50f;
            const float borderWidth = 3f;
            _game = game;
            IGameFactory factory = game.Factory;
            _panel = factory.UI.GetPanel(_panelId, 800, 600, 600f, 400f);
			_panel.Anchor = new PointF(0.5f, 0.5f);
			_panel.Visible = false;
            _panel.Tint = Colors.Black;
            _panel.Border = AGSBorders.SolidColor(Colors.Green, borderWidth, hasRoundCorners: true);
            _panel.RenderLayer = _layer;
			_panel.AddComponent<IModalWindowComponent>();

            var headerLabel = factory.UI.GetLabel("FeaturesHeaderLabel", "Guided Tour", _panel.Width, headerHeight, 0f, _panel.Height - headerHeight,
                                                  new AGSTextConfig(alignment: Alignment.MiddleCenter, autoFit: AutoFit.TextShouldFitLabel));
            headerLabel.TreeNode.SetParent(_panel.TreeNode);
            headerLabel.Tint = Colors.Transparent;
            headerLabel.Border = _panel.Border;
            headerLabel.RenderLayer = _layer;

            var xButton = factory.UI.GetButton("FeaturesCloseButton", (IAnimation)null, null, null, 0f, _panel.Height - headerHeight + 5f, "X", 
                                               new AGSTextConfig(factory.Graphics.Brushes.LoadSolidBrush(Colors.Red),
                                                                 autoFit: AutoFit.TextShouldFitLabel, alignment: Alignment.MiddleCenter), 
                                                                 width: 40f, height: 40f);
            xButton.Anchor = new PointF();
            xButton.TreeNode.SetParent(_panel.TreeNode);
            xButton.RenderLayer = _layer;
            xButton.Tint = Colors.Transparent;
            xButton.MouseEnter.Subscribe((_, __) => xButton.TextConfig = AGSTextConfig.ChangeColor(xButton.TextConfig, Colors.Yellow, Colors.White, 0.3f));
            xButton.MouseLeave.Subscribe((_, __) => xButton.TextConfig = AGSTextConfig.ChangeColor(xButton.TextConfig, Colors.Red, Colors.Transparent, 0f));
            xButton.OnMouseClick(hide, _game);

            var sidePanel = factory.UI.GetPanel("FeaturesSidePanel", _panel.Width / 4f, _panel.Height - headerHeight - borderWidth, 0f, 0f); 
            sidePanel.TreeNode.SetParent(_panel.TreeNode);
            sidePanel.Tint = _panel.Tint;
            sidePanel.RenderLayer = _layer;
            sidePanel.Border = _panel.Border;

            var treePanel = factory.UI.GetPanel("FeaturesTreePanel", 1f, 1f, 0f, _panel.Height - headerHeight - 40f);
            treePanel.Tint = Colors.Transparent;
            treePanel.TreeNode.SetParent(sidePanel.TreeNode);
            treePanel.RenderLayer = _layer;

            var tree = createFeaturesLabel("Features", null);

            var treeView = treePanel.AddComponent<ITreeViewComponent>();

            var uiLabel = createFeaturesLabel("GUIs", tree);
            createFeaturesLabel("Labels", uiLabel);
            createFeaturesLabel("Buttons", uiLabel);
            createFeaturesLabel("Skins", uiLabel);

            var objLabel = createFeaturesLabel("Objects", tree);
            createFeaturesLabel("Animations", objLabel);
            createFeaturesLabel("Transforms", objLabel);

            treeView.Tree = tree;

            _game.Events.OnSavedGameLoad.Subscribe((sender, args) => findPanel());
        }

		public void Show()
		{
			_lastMode = _scheme.CurrentMode;
			_scheme.CurrentMode = MouseCursors.POINT_MODE;
			_scheme.RotatingEnabled = false;
			_panel.Visible = true;
			_panel.GetComponent<IModalWindowComponent>().GrabFocus();
		}

        private ITreeStringNode createFeaturesLabel(string text, ITreeStringNode parent)
        {
            var node = new AGSTreeStringNode { Text = text };
            if (parent != null) node.TreeNode.SetParent(parent.TreeNode);
            return node;
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

		private void findPanel()
		{
			_panel = _game.Find<IPanel>(_panelId);
		}
    }
}
