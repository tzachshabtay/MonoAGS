using System;
using System.Drawing;

namespace AGS.API
{
	public interface IImage
	{
		Bitmap OriginalBitmap { get; }

		float Width { get; }
		float Height { get; }

		//A unique ID- can be the file path assuming it's not a sprite sheet
		string ID { get; }

		//null if image was not from a sprite sheet
		ISpriteSheet SpriteSheet { get; }

		ILoadImageConfig LoadConfig { get; }
	}
}

