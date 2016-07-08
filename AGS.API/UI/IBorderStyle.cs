namespace AGS.API
{
    public interface IBorderStyle
	{
		void RenderBorderBack(ISquare square);
		void RenderBorderFront(ISquare square);

        float WidthLeft { get; }
        float WidthRight { get; }
        float WidthTop { get; }
        float WidthBottom { get; }
	}
}

