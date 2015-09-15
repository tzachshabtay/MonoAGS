using System;
using AGS.API;

namespace AGS.Engine
{
	public class AGSSingleFrameAnimation : AGSAnimation
	{
		public AGSSingleFrameAnimation (IImage image) : this(getDefaultSprite(image))
		{
		}

		public AGSSingleFrameAnimation (ISprite sprite) : base(new AGSAnimationConfiguration { Loops = 1 },
			new AGSAnimationState(), 1)
		{
			AGSAnimationFrame frame = new AGSAnimationFrame (sprite) { Delay = -1 };
			Frames.Add (frame);
			Setup ();
		}

		private static ISprite getDefaultSprite(IImage image)
		{
			return new AGSSprite { Image = image, Location = new AGSLocation() };
		}
	}
}

