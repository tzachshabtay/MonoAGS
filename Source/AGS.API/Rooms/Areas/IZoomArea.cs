namespace AGS.API
{
    [RequiredComponent(typeof(IArea))]
    public interface IZoomArea : IComponent
    {
        float MinZoom { get; set; }
        float MaxZoom { get; set; }
        bool ZoomCamera { get; set; }

        float GetZoom(float value);
    }
}
