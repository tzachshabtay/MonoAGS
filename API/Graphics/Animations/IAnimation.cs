using System;
using System.Collections.Generic;

namespace AGS.API
{
	public interface IAnimation
	{
		IList<IAnimationFrame> Frames { get; }

		IAnimationConfiguration Configuration { get; }

		IAnimationState State { get; }

		ISprite Sprite { get; }

		void NextFrame();

		void FlipHorizontally();

		void FlipVertically();

		IAnimation Clone();
	}
}

