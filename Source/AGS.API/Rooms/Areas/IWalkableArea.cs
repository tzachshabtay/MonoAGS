namespace AGS.API
{
    /// <summary>
    /// These are the areas that indicate where the characters can walk.
    /// </summary>
    [RequiredComponent(typeof(IArea))]
    public interface IWalkableArea : IComponent
    {
        /// <summary>
        /// Gets or sets a value indicating whether this area is walkable.
        /// Note that you can also turn the <see cref="IEnabledComponent.Enabled"/>  on/off
        /// to achieve the same effect.The difference is that if you have an area which is both a walkable area but 
        /// also is a different kind an area(areas can have multiple roles) than <see cref="IEnabledComponent.Enabled"/> 
        /// controls all of area roles, and `IsWalkable` only affects the area's walk-ability.
        /// </summary>
        /// <value><c>true</c> if is walkable; otherwise, <c>false</c>.</value>
        bool IsWalkable { get; set; }
    }
}
