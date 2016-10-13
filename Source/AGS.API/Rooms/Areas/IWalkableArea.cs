namespace AGS.API
{
    [RequiredComponent(typeof(IArea))]
    public interface IWalkableArea : IComponent
    {
        bool IsWalkable { get; set; }
    }
}
