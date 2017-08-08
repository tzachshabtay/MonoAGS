using AGS.API;

namespace AGS.Engine
{
	public class AGSDrawableInfoComponent : AGSComponent, IDrawableInfo
	{
        private bool _ignoreScalingArea, _ignoreViewport;
        private IRenderLayer _renderLayer;

		public AGSDrawableInfoComponent()
		{
            OnIgnoreScalingAreaChanged = new AGSEvent<object>();
            OnIgnoreViewportChanged = new AGSEvent<object>();
            OnRenderLayerChanged = new AGSEvent<object>();
		}

        public IRenderLayer RenderLayer { get { return _renderLayer; } set { _renderLayer = value; OnRenderLayerChanged.FireEvent(null); } }

        public bool IgnoreViewport { get { return _ignoreViewport; } set { _ignoreViewport = value; OnIgnoreViewportChanged.FireEvent(null);} }

        public bool IgnoreScalingArea { get { return _ignoreScalingArea; } set { _ignoreScalingArea = value; OnIgnoreScalingAreaChanged.FireEvent(null); } }

        public IEvent<object> OnIgnoreScalingAreaChanged { get; private set; }

        public IEvent<object> OnIgnoreViewportChanged { get; private set; }

        public IEvent<object> OnRenderLayerChanged { get; private set; }
	}
}

