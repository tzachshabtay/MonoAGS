using AGS.API;

namespace AGS.Engine
{
	public class AGSScalingArea : AGSComponent, IScalingArea
	{
        private IAreaComponent _area;

        public AGSScalingArea()
        {
            Axis = ScalingAxis.Y;
            MinScaling = 1f;
            MaxScaling = 1f;
        }

		public override void Init(IEntity entity)
        {
            base.Init(entity);
            _area = entity.GetComponent<IAreaComponent>();
        }

        #region IScalingArea implementation

        public float GetScaling(float value)
        {
            if (Axis == ScalingAxis.X) 
                return MathUtils.Lerp(_area.Mask.MinX, MinScaling, _area.Mask.MaxX, MaxScaling, value);
            return MathUtils.Lerp(_area.Mask.MaxY, MinScaling, _area.Mask.MinY, MaxScaling, value);
        }

		public float MinScaling { get; set; }

		public float MaxScaling { get; set; }

		public bool ScaleObjectsX { get; set; }

        public bool ScaleObjectsY { get; set; }

		public bool ScaleVolume { get; set; }

        public ScalingAxis Axis { get; set; }

		#endregion
	}
}

