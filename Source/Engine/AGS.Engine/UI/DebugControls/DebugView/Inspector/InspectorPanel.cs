using System.ComponentModel;
using AGS.API;

namespace AGS.Engine
{
    public class InspectorPanel
    {
		private readonly IRenderLayer _layer;
		private readonly IGame _game;
        private IPanel _panel, _scrollingPanel, _parent;
        private ITextBox _searchBox;
        const float _padding = 10f;

        public InspectorPanel(IGame game, IRenderLayer layer)
        {
            _game = game;
            _layer = layer;
        }

        public AGSInspector Inspector { get; private set; }

        public IPanel Panel => _scrollingPanel;

        public void Load(IPanel parent)
        {
            _parent = parent;
			var factory = _game.Factory;

            _searchBox = factory.UI.GetTextBox("GameDebugInspectorSearchBox", 0f, parent.Height, parent, "Search...", width: parent.Width, height: 30f);
            _searchBox.RenderLayer = _layer;
            _searchBox.Border = AGSBorders.SolidColor(Colors.Green, 2f);
            _searchBox.Tint = Colors.Transparent;
            _searchBox.Pivot = new PointF(0f, 1f);
            _searchBox.GetComponent<ITextComponent>().PropertyChanged += onSearchPropertyChanged;

            var height = parent.Height - _searchBox.Height;
            _scrollingPanel = factory.UI.GetPanel("GameDebugInspectorScrollingPanel", parent.Width, height, 0f, height, parent);
			_scrollingPanel.RenderLayer = _layer;
			_scrollingPanel.Pivot = new PointF(0f, 1f);
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
            _parent.Bind<IScaleComponent>(c => c.PropertyChanged += onParentPanelScaleChanged,
                                          c => c.PropertyChanged -= onParentPanelScaleChanged);
        }

		public void Resize()
		{
			_scrollingPanel.Image = new EmptyImage(_parent.Width, _scrollingPanel.Image.Height);
            _searchBox.LabelRenderSize = new SizeF(_parent.Width, _searchBox.Height);
		}

        private void onParentPanelScaleChanged(object sender, PropertyChangedEventArgs args)
        {
            if (args.PropertyName != nameof(IScaleComponent.Height)) return;
            _scrollingPanel.Image = new EmptyImage(_scrollingPanel.Image.Width, _parent.Height - _searchBox.Height);
            _scrollingPanel.Y = _parent.Height - _searchBox.Height;
            _searchBox.Y = _parent.Height;
            _panel.Y = _scrollingPanel.Height - _padding;
        }

        private void onSearchPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName != nameof(ITextComponent.Text)) return;
            Inspector.Tree.SearchFilter = _searchBox.Text;
        }
    }
}
