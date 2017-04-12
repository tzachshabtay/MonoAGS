namespace AGS.API
{
    public interface IHasImage
    {
        byte Opacity { get; set; }
        Color Tint { get; set; }
        PointF Anchor { get; set; }

        IImage Image { get; set; }
        IImageRenderer CustomRenderer { get; set; }

        IEvent<AGSEventArgs> OnImageChanged { get; }
        IEvent<AGSEventArgs> OnAnchorChanged { get; }
        IEvent<AGSEventArgs> OnTintChanged { get; }
    }

    [RequiredComponent(typeof(IAnimationContainer))]
    [RequiredComponent(typeof(IScaleComponent))]
    public interface IImageComponent : IHasImage, IComponent
    {
    }
}
