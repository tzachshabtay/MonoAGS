namespace AGS.API
{
    public interface ISprite : IHasImage, ITransform, IScale, IPixelPerfectCollidable, IRotate
	{
        ISprite Clone();
	}
}

