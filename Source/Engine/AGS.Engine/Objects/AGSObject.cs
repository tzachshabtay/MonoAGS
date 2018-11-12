namespace AGS.Engine
{
	public partial class AGSObject
	{
		partial void afterInitComponents(Resolver resolver)
		{
			RenderLayer = AGSLayers.Foreground;
			IgnoreScalingArea = true;
		}
	}
}

