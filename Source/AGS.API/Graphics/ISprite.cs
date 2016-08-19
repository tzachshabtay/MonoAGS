namespace AGS.API
{
    public interface ISprite : IHasImage, ITranslate, IScale, IPixelPerfectCollidable, IRotate, 
        IHasModelMatrix
	{
        ISprite Clone();
	}

    public interface IHasModelMatrix : IScale, IRotate, ITranslate, IHasImage
    {
    }
}

