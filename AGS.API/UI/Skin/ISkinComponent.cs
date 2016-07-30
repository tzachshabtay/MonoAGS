namespace AGS.API
{
    public interface ISkinComponent : IComponent
    {
        ISkin Skin { get; set; }
        IConcurrentHashSet<string> SkinTags { get; }
    }
}
