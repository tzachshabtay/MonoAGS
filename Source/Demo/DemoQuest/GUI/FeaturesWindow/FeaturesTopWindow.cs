using System;
using System.Collections.Generic;
using AGS.API;
using AGS.Engine;

namespace DemoGame
{
    public class FeaturesTopWindow
    {
		private const string _panelId = "Features Panel";
		private IPanel _panel, _rightSidePanel;
		private IGame _game;

		private string _lastMode;
		private readonly RotatingCursorScheme _scheme;
        private readonly IRenderLayer _layer;
        private Dictionary<string, Lazy<IFeaturesPanel>> _panels;
        private IFeaturesPanel _currentPanel;

		public FeaturesTopWindow(RotatingCursorScheme scheme)
		{
			_scheme = scheme;
            _layer = new AGSRenderLayer(AGSLayers.UI.Z, independentResolution: (1200, 800));
            _panels = new Dictionary<string, Lazy<IFeaturesPanel>>();
		}

        public void Load(IGame game)
        {
            const float headerHeight = 50f;
            const float borderWidth = 3f;
            _game = game;
            IGameFactory factory = game.Factory;
            _panel = factory.UI.GetPanel(_panelId, 800, 600, 
                 _layer.IndependentResolution.Value.Width / 2f, _layer.IndependentResolution.Value.Height / 2f);
			_panel.Pivot = (0.5f, 0.5f);
			_panel.Visible = false;
            _panel.Tint = Colors.Black;
            _panel.Border = factory.Graphics.Borders.SolidColor(Colors.Green, borderWidth, hasRoundCorners: true);
            _panel.RenderLayer = _layer;
			_panel.AddComponent<IModalWindowComponent>();

            var headerLabel = factory.UI.GetLabel("FeaturesHeaderLabel", "Guided Tour", _panel.Width, headerHeight, 0f, _panel.Height - headerHeight,
                                                  _panel, new AGSTextConfig(alignment: Alignment.MiddleCenter, autoFit: AutoFit.TextShouldFitLabel));
            headerLabel.Tint = Colors.Transparent;
            headerLabel.Border = _panel.Border;
            headerLabel.RenderLayer = _layer;

            var xButton = factory.UI.GetButton("FeaturesCloseButton", (IAnimation)null, null, null, 0f, _panel.Height - headerHeight + 5f, _panel, "X", 
                                               new AGSTextConfig(factory.Graphics.Brushes.LoadSolidBrush(Colors.Red),
                                                                 autoFit: AutoFit.TextShouldFitLabel, alignment: Alignment.MiddleCenter), 
                                                                 width: 40f, height: 40f);
            xButton.Pivot = (0f, 0f);
            xButton.RenderLayer = _layer;
            xButton.Tint = Colors.Transparent;
            xButton.MouseEnter.Subscribe(_ => xButton.TextConfig = AGSTextConfig.ChangeColor(xButton.TextConfig, Colors.Yellow, Colors.White, 0.3f));
            xButton.MouseLeave.Subscribe(_ => xButton.TextConfig = AGSTextConfig.ChangeColor(xButton.TextConfig, Colors.Red, Colors.Transparent, 0f));
            xButton.OnMouseClick(hide, _game);

            var leftSidePanel = factory.UI.GetPanel("FeaturesLeftSidePanel", _panel.Width / 4f, _panel.Height - headerHeight - borderWidth, 0f, 0f, _panel); 
            leftSidePanel.Tint = Colors.Transparent;
            leftSidePanel.RenderLayer = _layer;
            leftSidePanel.Border = _panel.Border;

            _rightSidePanel = factory.UI.GetPanel("FeaturesRightSidePanel", _panel.Width - leftSidePanel.Width, leftSidePanel.Height, leftSidePanel.Width, 0f, _panel);
            _rightSidePanel.RenderLayer = _layer;
            _rightSidePanel.Tint = Colors.Green.WithAlpha(50);

            var treePanel = factory.UI.GetPanel("FeaturesTreePanel", 1f, 1f, 0f, _panel.Height - headerHeight - 40f, leftSidePanel);
            treePanel.Tint = Colors.Transparent;
            treePanel.RenderLayer = _layer;

            var tree = createFeaturesLabel("Features", null);

            var treeView = treePanel.AddComponent<ITreeViewComponent>();

            var roomsLabel = createFeaturesLabel("Rooms", tree);
            createFeaturesLabel("Viewports", roomsLabel, () => new FeaturesViewportsPanel(_game, _rightSidePanel));
            createFeaturesLabel("Moving Areas", roomsLabel, () => new FeaturesMoveAreaPanel(_game, _rightSidePanel, _scheme));

            var uiLabel = createFeaturesLabel("GUIs", tree);
            createFeaturesLabel("Labels", uiLabel, () => new FeaturesLabelsPanel(_game, _rightSidePanel));

            var objLabel = createFeaturesLabel("Objects", tree);
            createFeaturesLabel("Textures", objLabel, () => new FeaturesTexturesPanel(_game, _rightSidePanel));
            createFeaturesLabel("Tweens", objLabel, () => new FeaturesTweenPanel(_game, _rightSidePanel, _panel));

            treeView.Tree = tree;
            treeView.OnNodeSelected.Subscribe(onTreeNodeSelected);

            _game.Events.OnSavedGameLoad.Subscribe(findPanel);
        }

		public void Show()
		{
			_lastMode = _scheme.CurrentMode;
			_scheme.CurrentMode = MouseCursors.POINT_MODE;
			_scheme.RotatingEnabled = false;
			_panel.Visible = true;
			_panel.GetComponent<IModalWindowComponent>().GrabFocus();
		}

        private async void onTreeNodeSelected(NodeEventArgs args)
        {
            var current = _currentPanel;
            if (current != null) await current.Close();
            if (!_panels.TryGetValue(args.Node.Text, out var panel) || panel == null) return;
            panel.Value.Show();
            _currentPanel = panel.Value;
        }

        private ITreeStringNode createFeaturesLabel(string text, ITreeStringNode parent, Func<IFeaturesPanel> panel = null)
        {
            var node = new AGSTreeStringNode(text, null);
            if (parent != null) node.TreeNode.SetParent(parent.TreeNode);
            _panels[text] = panel == null ? null : new Lazy<IFeaturesPanel>(panel);
            return node;
        }

        private async void hide()
		{
            var currentPanel = _currentPanel;
            if (currentPanel != null) await currentPanel.Close();
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
