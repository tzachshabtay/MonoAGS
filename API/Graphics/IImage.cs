using System;
using System.Drawing;

namespace API
{
	public interface IImage
	{
		Bitmap OriginalBitmap { get; }

		float Width { get; }
		float Height { get; }

		//A unique ID- can be the file path assuming it's not a sprite sheet
		string ID { get; }
	}
}

