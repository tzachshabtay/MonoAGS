namespace AGS.API
{
    public interface ISprite : IHasImage, ITransform
	{
		IArea PixelPerfectHitTestArea { get; } //This is the area not projected on the screen (i.e not rotated and will start from 0,0)
		void PixelPerfect(bool pixelPerfect);

		float Height { get; }
		float Width { get; }
		float ScaleX { get; }
		float ScaleY { get; }
		float Angle { get; set; }

        void ResetScale();
		void ScaleBy(float scaleX, float scaleY);
		void ScaleTo(float width, float height);
		void FlipHorizontally();
		void FlipVertically();
		ISprite Clone();
	}
}

