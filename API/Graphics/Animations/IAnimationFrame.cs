using System;

namespace API
{
	public interface IAnimationFrame
	{
		ISprite Sprite { get; set; }
		int Delay { get; set; }
		ISound Sound { get; set; }
	}
}

