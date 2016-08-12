namespace AGS.API
{
    public interface IScale
    {
        float Height { get; }
        float Width { get; }
        float ScaleX { get; }
        float ScaleY { get; }

        void ResetScale();
        void ScaleBy(float scaleX, float scaleY);
        void ScaleTo(float width, float height);
        void FlipHorizontally();
        void FlipVertically();
    }

    [RequiredComponent(typeof(IImageComponent))]
    public interface IScaleComponent : IScale, IComponent
    {
    }
}
