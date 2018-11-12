using AGS.API;

namespace AGS.Engine
{
	public class AGSSingleFrameAnimation : AGSAnimation
	{
		public AGSSingleFrameAnimation (IImage image, IGraphicsFactory factory) : this(getDefaultSprite(image, factory))
		{
		}

		public AGSSingleFrameAnimation (ISprite sprite) : base(new AGSAnimationConfiguration { Loops = 1 },
			new AGSAnimationState(), 1)
		{
			AGSAnimationFrame frame = new AGSAnimationFrame (sprite) { Delay = -1 };
			Frames.Add (frame);
			Setup ();
		}

		private static ISprite getDefaultSprite(IImage image, IGraphicsFactory factory)
		{
			ISprite sprite = factory.GetSprite();
			sprite.Image = image;
            sprite.Position = Position.Empty;
			return sprite;
		}
	}
}