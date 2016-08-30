namespace AGS.API
{
    public interface ILabelRenderer : IImageRenderer
	{
		string Text { get; set; }
		ITextConfig Config { get; set; }
	    SizeF BaseSize { get; set; }

        int? CaretPosition { get; set; }

		float Width { get; }
		float Height { get; }

		float TextWidth { get; }
		float TextHeight { get; }
	}
}

