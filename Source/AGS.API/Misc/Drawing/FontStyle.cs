using System;

namespace AGS.API
{
    /// <summary>
    /// Font style (regular, bold, italic, underline or strikeout).
    /// </summary>
	[Flags]
	public enum FontStyle
	{
		Regular = 0,
		Bold = 1,
		Italic = 2,
		Underline = 4,
		Strikeout = 8
	}
}

