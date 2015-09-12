using System;

namespace API
{
	public interface IImage
	{
		float Width { get; }
		float Height { get; }

		//A unique ID- can be the file path assuming it's not a sprite sheet
		string ID { get; }
	}
}

