using AGS.API;

namespace AGS.Engine
{
    public class InspectorPanel
    {
		private readonly IRenderLayer _layer;
		private readonly IGame _game;
        private IPanel _panel, _scrollingPanel;

        public InspectorPanel(IGame game, IRenderLayer layer)
        {
            _game = game;
            _layer = layer;
        }

        public AGSInspector Inspector { get; private set; }

        public IPanel Panel { get { return _scrollingPanel; } }

        public void Load(IPanel parent)
        {
			var factory = _game.Factory;
            var height = parent.Height / 4f;
            _scrollingPanel = factory.UI.GetPanel("GameDebugInspectorScrollingPanel", parent.Width, height, 0f, height, parent);
			_scrollingPanel.RenderLayer = _layer;
			_scrollingPanel.Anchor = new PointF(0f, 1f);
			_scrollingPanel.Tint = Colors.Transparent;
			_scrollingPanel.Border = AGSBorders.SolidColor(Colors.Green, 2f);

            _panel = factory.UI.GetPanel("GameDebugInspectorPanel", parent.Width, height - 42f, 0f, height - 42f, _scrollingPanel);
			_panel.Tint = Colors.Transparent;
			_panel.RenderLayer = _layer;
			_panel.AddComponent<ITreeViewComponent>();

            Inspector = new AGSInspector();
            _panel.AddComponent(Inspector);
            factory.UI.CreateScollingPanel(_scrollingPanel);
			_scrollingPanel.OnScaleChanged.Subscribe(() =>
			{
				_panel.Y = _scrollingPanel.Height - 42f;
			});
        }
    }
}
