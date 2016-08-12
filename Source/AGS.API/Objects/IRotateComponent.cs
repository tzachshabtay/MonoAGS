namespace AGS.API
{
    public interface IRotate
    {
        float Angle { get; set; }
    }

    public interface IRotateComponent : IRotate, IComponent
    {
    }
}
