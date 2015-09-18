using System;
using System.Drawing;

namespace AGS.API
{
	public interface ILabelRenderer : IImageRenderer
	{
		string Text { get; set; }
		ITextConfig Config { get; set; }
	    SizeF BaseSize { get; set; }
	}
}

