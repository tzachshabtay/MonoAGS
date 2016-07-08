namespace AGS.API
{
    public interface ISayLocationProvider
	{
        ISayLocation GetLocation(string text, ISayConfig config);
	}
}

