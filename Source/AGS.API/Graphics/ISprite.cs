namespace AGS.API
{
    public interface ISprite : IHasImage, ITransform, IScale
	{
		IArea PixelPerfectHitTestArea { get; } //This is the area not projected on the screen (i.e not rotated and will start from 0,0)
		void PixelPerfect(bool pixelPerfect);

		float Angle { get; set; }

        ISprite Clone();
	}
}

