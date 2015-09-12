using System;

namespace API
{
	public interface ILabelRenderer : IImageRenderer
	{
		string Text { get; set; }
		ITextConfig Config { get; set; }
	}
}

