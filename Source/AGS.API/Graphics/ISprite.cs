namespace AGS.API
{
    public interface ISprite : IHasImage, ITransform, IScale, IPixelPerfectCollidable, IRotate, 
        IHasModelMatrix
	{
        ISprite Clone();
	}

    public interface IHasModelMatrix : IScale, IRotate, ITransform, IHasImage
    {
    }
}

