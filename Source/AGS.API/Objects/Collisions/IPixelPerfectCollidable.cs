namespace AGS.API
{
    public interface IPixelPerfectCollidable
    {
        IArea PixelPerfectHitTestArea { get; } //This is the area not projected on the screen (i.e not rotated and will start from 0,0)
        void PixelPerfect(bool pixelPerfect);
    }

    [RequiredComponent(typeof(IAnimationContainer))]
    public interface IPixelPerfectComponent : IPixelPerfectCollidable, IComponent
    { }
}
