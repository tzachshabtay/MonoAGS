namespace AGS.API
{
    /// <summary>
    /// Allows skinning (providing a common custom theme to) an entity.
    /// It's usually used for GUIs.
    /// For example, you might have a blue themed skin which gives all controls a common blue-ish look.
    /// </summary>
    public interface ISkinComponent : IComponent
    {
        /// <summary>
        /// Gets or sets the skin used by the entity.
        /// </summary>
        /// <value>The skin.</value>
        ISkin Skin { get; set; }

        /// <summary>
        /// A collection of tags that can be used by the skin to distinguish between different entities.
        /// </summary>
        /// <value>The skin tags.</value>
        IConcurrentHashSet<string> SkinTags { get; }
    }
}
