namespace AGS.API
{
    [RequiredComponent(typeof(IArea))]
    public interface IScalingArea : IComponent
	{
		float MinScaling { get; set; }
		float MaxScaling { get; set; }
		bool ScaleObjects { get; set; }
		bool ScaleVolume { get; set; }
        
        float GetScaling(float value);
	}
}

