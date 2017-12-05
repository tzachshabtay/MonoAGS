namespace AGS.API
{
    /// <summary>
    /// Allows rotating an entity/sprite.
    /// </summary>
    public interface IRotate
    {
        /// <summary>
        /// Gets or sets the angle of the entity/sprite (in degrees).
        /// </summary>
        /// <value>The angle.</value>
        float Angle { get; set; }
    }

    /// <summary>
    /// A component which allows rotating an entity.
    /// </summary>
    public interface IRotateComponent : IRotate, IComponent
    {
    }
}
