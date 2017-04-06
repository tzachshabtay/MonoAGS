using AGS.API;

namespace AGS.Engine
{
	public class AGSDrawableInfoComponent : AGSComponent, IDrawableInfo
	{
        private readonly AGSEventArgs _ignoreScalingArgs = new AGSEventArgs();
        private readonly AGSEventArgs _renderLayerArgs = new AGSEventArgs();
        private bool _ignoreScalingArea;
        private IRenderLayer _renderLayer;

		public AGSDrawableInfoComponent()
		{
            OnIgnoreScalingAreaChanged = new AGSEvent<AGSEventArgs>();
            OnRenderLayerChanged = new AGSEvent<AGSEventArgs>();
		}

        public IRenderLayer RenderLayer { get { return _renderLayer; } set { _renderLayer = value; OnRenderLayerChanged.FireEvent(this, _renderLayerArgs); } }

		public bool IgnoreViewport { get; set; }

        public bool IgnoreScalingArea { get { return _ignoreScalingArea; } set { _ignoreScalingArea = value; OnIgnoreScalingAreaChanged.FireEvent(this, _ignoreScalingArgs); } }

        public IEvent<AGSEventArgs> OnIgnoreScalingAreaChanged { get; private set; }

        public IEvent<AGSEventArgs> OnRenderLayerChanged { get; private set; }
	}
}

