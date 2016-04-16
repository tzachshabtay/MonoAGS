using System;

namespace AGS.API
{
	public interface IFont
	{
		string FontFamily { get; }
		FontStyle Style { get; }
		float SizeInPoints { get; }

		SizeF MeasureString(string text, int maxWidth = int.MaxValue);
	}
}

