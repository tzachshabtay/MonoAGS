namespace AGS.API
{
    public interface IEdge
	{
		float Value { get; set; }
		IEvent<AGSEventArgs> OnEdgeCrossed { get; }
        bool Enabled { get; set; }
	}
}

