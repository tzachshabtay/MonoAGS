using System;

namespace AGS.API
{
	public interface IBitmapTextDraw
	{
		void DrawText(string text, ITextConfig config, AGS.API.SizeF textSize, AGS.API.SizeF baseSize, 
			int maxWidth, int height);
	}
}

