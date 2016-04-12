using System;

namespace AGS.API
{
	public interface IFont
	{
		SizeF MeasureString(string text, int maxWidth = int.MaxValue);
	}
}

