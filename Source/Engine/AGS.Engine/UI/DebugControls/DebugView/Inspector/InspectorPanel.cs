using AGS.API;

namespace AGS.Engine
{
    public class InspectorPanel
    {
		private readonly IRenderLayer _layer;
		private readonly IGame _game;
        private IPanel _panel;

        public InspectorPanel(IGame game, IRenderLayer layer)
        {
            _game = game;
            _layer = layer;
        }

        public AGSInspector Inspector { get; private set; }

        public void Load(IPanel parent)
        {
			var factory = _game.Factory;
            var height = parent.Height / 4f;
            _panel = factory.UI.GetPanel("GameDebugInspectorPanel", parent.Width, height, 0f, height, parent);
			_panel.Tint = Colors.Transparent;
			_panel.RenderLayer = _layer;
			_panel.AddComponent<ITreeViewComponent>();
            Inspector = new AGSInspector();
            _panel.AddComponent(Inspector);
        }
    }
}
