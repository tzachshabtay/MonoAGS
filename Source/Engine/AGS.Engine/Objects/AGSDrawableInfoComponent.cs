using AGS.API;

namespace AGS.Engine
{
	public class AGSDrawableInfoComponent : AGSComponent, IDrawableInfo
	{
        private bool _ignoreScalingArea;
        private IRenderLayer _renderLayer;

		public AGSDrawableInfoComponent()
		{
            OnIgnoreScalingAreaChanged = new AGSEvent<object>();
            OnRenderLayerChanged = new AGSEvent<object>();
		}

        public IRenderLayer RenderLayer { get { return _renderLayer; } set { _renderLayer = value; OnRenderLayerChanged.FireEvent(null); } }

		public bool IgnoreViewport { get; set; }

        public bool IgnoreScalingArea { get { return _ignoreScalingArea; } set { _ignoreScalingArea = value; OnIgnoreScalingAreaChanged.FireEvent(null); } }

        public IEvent<object> OnIgnoreScalingAreaChanged { get; private set; }

        public IEvent<object> OnRenderLayerChanged { get; private set; }
	}
}

