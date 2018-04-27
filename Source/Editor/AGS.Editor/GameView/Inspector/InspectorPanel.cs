using System.ComponentModel;
using AGS.API;
using AGS.Engine;
using GuiLabs.Undo;

namespace AGS.Editor
{
    public class InspectorPanel
    {
		private readonly IRenderLayer _layer;
		private readonly IGame _game, _editor;
        private readonly ActionManager _actions;
        private IPanel _treePanel, _scrollingPanel, _contentsPanel, _parent;
        private ITextBox _searchBox;
        private InspectorTreeNodeProvider _inspectorNodeView;

        const float _padding = 42f;
        const float _gutterSize = 15f;

        public InspectorPanel(IGame editor, IGame game, IRenderLayer layer, ActionManager actions)
        {
            _editor = editor;
            _actions = actions;
            _game = game;
            _layer = layer;
        }

        public AGSInspector Inspector { get; private set; }

        public IPanel Panel => _scrollingPanel;

        public void Load(IPanel parent)
        {
            _parent = parent;
            var factory = _editor.Factory;

            _searchBox = factory.UI.GetTextBox("GameDebugInspectorSearchBox", 0f, parent.Height, parent, "Search...", width: parent.Width, height: 30f);
            _searchBox.RenderLayer = _layer;
            _searchBox.Border = AGSBorders.SolidColor(GameViewColors.Border, 2f);
            _searchBox.Tint = GameViewColors.Textbox;
            _searchBox.Pivot = new PointF(0f, 1f);
            _searchBox.GetComponent<ITextComponent>().PropertyChanged += onSearchPropertyChanged;

            var height = parent.Height - _searchBox.Height - _gutterSize;
            _scrollingPanel = factory.UI.GetPanel("GameDebugInspectorScrollingPanel", parent.Width - _gutterSize, height, 0f, parent.Height - _searchBox.Height, parent);
			_scrollingPanel.RenderLayer = _layer;
			_scrollingPanel.Pivot = new PointF(0f, 1f);
			_scrollingPanel.Tint = Colors.Transparent;
            _scrollingPanel.Border = AGSBorders.SolidColor(GameViewColors.Border, 2f);
            _contentsPanel = factory.UI.CreateScrollingPanel(_scrollingPanel);

            _treePanel = factory.UI.GetPanel("GameDebugInspectorPanel", 0f, 0f, 0f, _contentsPanel.Height - _padding, _contentsPanel);
			_treePanel.Tint = Colors.Transparent;
            _treePanel.RenderLayer = _layer;
            _treePanel.Pivot = new PointF(0f, 1f);
			var treeView = _treePanel.AddComponent<ITreeViewComponent>();
            treeView.SkipRenderingRoot = true;

            Inspector = new AGSInspector(_editor.Factory, _game.Settings, _editor.State, _actions);
            _treePanel.AddComponent<IInspectorComponent>(Inspector);

            _inspectorNodeView = new InspectorTreeNodeProvider(treeView.NodeViewProvider, _editor.Factory,
                                                               _editor.Events, _treePanel);
            _inspectorNodeView.Resize(_contentsPanel.Width);
            treeView.NodeViewProvider = _inspectorNodeView;

            _parent.Bind<IScaleComponent>(c => c.PropertyChanged += onParentPanelScaleChanged,
                                          c => c.PropertyChanged -= onParentPanelScaleChanged);
        }

		public void Resize()
		{
            _contentsPanel.BaseSize = new SizeF(_parent.Width - _gutterSize, _contentsPanel.Height);
            _searchBox.LabelRenderSize = new SizeF(_parent.Width, _searchBox.Height);
            _searchBox.Watermark.LabelRenderSize = new SizeF(_parent.Width, _searchBox.Height);
            _inspectorNodeView.Resize(_contentsPanel.Width);
		}

        private void onParentPanelScaleChanged(object sender, PropertyChangedEventArgs args)
        {
            if (args.PropertyName != nameof(IScaleComponent.Height)) return;

            _contentsPanel.BaseSize = new SizeF(_contentsPanel.Width, _parent.Height - _searchBox.Height - _gutterSize);
            _scrollingPanel.Y = _parent.Height - _searchBox.Height;
            _searchBox.Y = _parent.Height;
            _treePanel.Y = _contentsPanel.Height - _padding;
        }

        private void onSearchPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName != nameof(ITextComponent.Text)) return;
            Inspector.Tree.SearchFilter = _searchBox.Text;
        }
    }
}
