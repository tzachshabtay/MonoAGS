using System;
using System.Collections.Generic;

namespace API
{
	public interface IAnimation
	{
		IList<IAnimationFrame> Frames { get; }

		IAnimationConfiguration Configuration { get; }

		IAnimationState State { get; }

		ISprite Sprite { get; }

		void NextFrame();

	}
}

