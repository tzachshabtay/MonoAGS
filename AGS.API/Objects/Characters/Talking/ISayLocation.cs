namespace AGS.API
{
    public interface ISayLocation
	{
		PointF GetLocation(string text, SizeF labelSize, ITextConfig config);
	}
}

