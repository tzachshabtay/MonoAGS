namespace AGS.Engine
{
	public partial class AGSObject
	{
	    // ReSharper disable once UnusedParameterInPartialMethod
	    partial void afterInitComponents(Resolver resolver)
		{
			RenderLayer = AGSLayers.Foreground;
			IgnoreScalingArea = true;
		}
	}
}

