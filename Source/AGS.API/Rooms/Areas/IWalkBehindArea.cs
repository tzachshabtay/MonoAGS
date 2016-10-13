namespace AGS.API
{
    [RequiredComponent(typeof(IArea))]
    public interface IWalkBehindArea : IComponent
	{
		float? Baseline { get; set; }
	}
}

