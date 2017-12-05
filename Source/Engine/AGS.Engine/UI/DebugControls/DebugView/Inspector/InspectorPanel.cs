using System.ComponentModel;
using AGS.API;

namespace AGS.Engine
{
    public class InspectorPanel
    {
		private readonly IRenderLayer _layer;
		private readonly IGame _game;
        private IPanel _panel, _scrollingPanel, _parent;
        const float _padding = 10f;

        public InspectorPanel(IGame game, IRenderLayer layer)
        {
            _game = game;
            _layer = layer;
        }

        public AGSInspector Inspector { get; private set; }

        public IPanel Panel { get { return _scrollingPanel; } }

        public void Load(IPanel parent)
        {
            _parent = parent;
			var factory = _game.Factory;
            var height = parent.Height / 2f;
            _scrollingPanel = factory.UI.GetPanel("GameDebugInspectorScrollingPanel", parent.Width, height, 0f, height, parent);
			_scrollingPanel.RenderLayer = _layer;
			_scrollingPanel.Anchor = new PointF(0f, 1f);
			_scrollingPanel.Tint = Colors.Transparent;
			_scrollingPanel.Border = AGSBorders.SolidColor(Colors.Green, 2f);

            _panel = factory.UI.GetPanel("GameDebugInspectorPanel", parent.Width, height - _padding, 0f, height - _padding, _scrollingPanel);
			_panel.Tint = Colors.Transparent;
			_panel.RenderLayer = _layer;
			var treeView = _panel.AddComponent<ITreeViewComponent>();
            treeView.NodeViewProvider = new InspectorTreeNodeProvider(treeView.NodeViewProvider, _game.Factory);

            Inspector = new AGSInspector(_game.Factory, _game.Settings);
            _panel.AddComponent<IInspectorComponent>(Inspector);
            factory.UI.CreateScrollingPanel(_scrollingPanel);
            _scrollingPanel.Bind<IScaleComponent>(c => c.PropertyChanged += onScrollingPanelScaleChanged,
                                                  c => c.PropertyChanged -= onScrollingPanelScaleChanged);
        }

		public void Resize()
		{
			_scrollingPanel.Image = new EmptyImage(_parent.Width, _scrollingPanel.Image.Height);
		}

        private void onScrollingPanelScaleChanged(object sender, PropertyChangedEventArgs args)
        {
            if (args.PropertyName != nameof(IScaleComponent.Height)) return;
            _panel.Y = _scrollingPanel.Height - _padding;
        }
    }
}
