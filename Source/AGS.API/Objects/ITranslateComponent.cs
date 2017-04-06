namespace AGS.API
{
    public interface ITranslate
    {
        ILocation Location { get; set; }
        float X { get; set; }
        float Y { get; set; }
        float Z { get; set; }
        IEvent<AGSEventArgs> OnLocationChanged { get; }
    }

    public interface ITranslateComponent : ITranslate, IComponent
    {        
    }
}
