namespace AGS.API
{
    public interface ITranslate
    {
        ILocation Location { get; set; }
        float X { get; set; }
        float Y { get; set; }
        float Z { get; set; }
    }

    public interface ITranslateComponent : ITranslate, IComponent
    {        
    }
}
