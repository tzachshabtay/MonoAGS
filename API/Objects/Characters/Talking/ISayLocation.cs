using System.Drawing;

namespace AGS.API
{
    public interface ISayLocation
	{
		IPoint GetLocation(string text, SizeF labelSize, ITextConfig config);
	}
}

