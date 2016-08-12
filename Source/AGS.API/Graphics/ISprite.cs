namespace AGS.API
{
    public interface ISprite : IHasImage, ITransform, IScale, IPixelPerfectCollidable
	{
		float Angle { get; set; }

        ISprite Clone();
	}
}

