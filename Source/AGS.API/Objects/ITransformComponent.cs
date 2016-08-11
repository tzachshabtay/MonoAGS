namespace AGS.API
{
    public interface ITransform
    {
        ILocation Location { get; set; }
        float X { get; set; }
        float Y { get; set; }
        float Z { get; set; }
    }

    public interface ITransformComponent : ITransform, IComponent
    {        
    }
}
