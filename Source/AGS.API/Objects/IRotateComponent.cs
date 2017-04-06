namespace AGS.API
{
    public interface IRotate
    {
        float Angle { get; set; }
        IEvent<AGSEventArgs> OnAngleChanged { get; }
    }

    public interface IRotateComponent : IRotate, IComponent
    {
    }
}
