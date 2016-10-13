using AGS.API;

namespace AGS.Engine
{
	public class AGSScalingArea : AGSComponent, IScalingArea
	{
        private IAreaComponent _area;

		public static void Create(IArea area, float minScaling, float maxScaling, bool scaleObjects = true, bool scaleVolume = true)
		{
            var component = area.AddComponent<IScalingArea>();
            component.MinScaling = minScaling;
            component.MaxScaling = maxScaling;
            component.ScaleObjects = scaleObjects;
            component.ScaleVolume = scaleVolume;
		}

		public override void Init(IEntity entity)
        {
            base.Init(entity);
            _area = entity.GetComponent<IAreaComponent>();
        }

        #region IScalingArea implementation

        public float GetScaling(float value)
        {
            return MathUtils.Lerp(_area.Mask.MaxY, MinScaling, _area.Mask.MinY, MaxScaling, value);
        }

		public float MinScaling { get; set; }

		public float MaxScaling { get; set; }

		public bool ScaleObjects { get; set; }

		public bool ScaleVolume { get; set; }

		#endregion
	}
}

