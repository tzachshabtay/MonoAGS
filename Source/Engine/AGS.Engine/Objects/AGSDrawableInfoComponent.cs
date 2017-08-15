using AGS.API;

namespace AGS.Engine
{
	public class AGSDrawableInfoComponent : AGSComponent, IDrawableInfo
	{
        private bool _ignoreScalingArea, _ignoreViewport;
        private IRenderLayer _renderLayer;

		public AGSDrawableInfoComponent()
		{
            OnIgnoreScalingAreaChanged = new AGSEvent();
            OnIgnoreViewportChanged = new AGSEvent();
            OnRenderLayerChanged = new AGSEvent();
		}

        public IRenderLayer RenderLayer { get { return _renderLayer; } set { _renderLayer = value; OnRenderLayerChanged.Invoke(); } }

        public bool IgnoreViewport { get { return _ignoreViewport; } set { _ignoreViewport = value; OnIgnoreViewportChanged.Invoke();} }

        public bool IgnoreScalingArea { get { return _ignoreScalingArea; } set { _ignoreScalingArea = value; OnIgnoreScalingAreaChanged.Invoke(); } }

        public IEvent OnIgnoreScalingAreaChanged { get; private set; }

        public IEvent OnIgnoreViewportChanged { get; private set; }

        public IEvent OnRenderLayerChanged { get; private set; }
	}
}

