namespace AGS.API
{
    /// <summary>
    /// Allows changing the position of entities/sprites.
    /// </summary>
    public interface ITranslate
    {
        /// <summary>
        /// Gets or sets the location.
        /// (X,Y) are used to place the entity/sprite in 2D space (in room coordinates).
        /// Z is used for deciding how the entities/sprites are ordered in 2D space (higher Z in front).
        /// If Z and Y are the same when setting the location, then Z will be bound to Y: whenever Y moves,
        /// Z moves, which will make entities closer to the bottom appear in front, which is the desired behavior
        /// in most scenarios. You can change this behavior by explicitly setting Z to a different value.
        /// </summary>
        /// <value>The location.</value>
        ILocation Location { get; set; }

        /// <summary>
        /// Gets or sets the x coordinate (in room coordinates).
        /// </summary>
        /// <value>The x.</value>
        float X { get; set; }

        /// <summary>
        /// Gets or sets the y coordinate (in room coordinates).
        /// </summary>
        /// <value>The y.</value>
        float Y { get; set; }

        /// <summary>
        /// Gets or sets the z property.
        /// Z is used for deciding how the entities/sprites are ordered in 2D space (higher Z in front).
        /// If Z and Y are the same when setting the location, then Z will be bound to Y: whenever Y moves,
        /// Z moves, which will make entities closer to the bottom appear in front, which is the desired behavior
        /// in most scenarios. You can change this behavior by explicitly setting Z to a different value.
        /// </summary>
        /// <value>The z.</value>
        float Z { get; set; }

        /// <summary>
        /// An event which fires whenever the location is changed.
        /// </summary>
        /// <value>The event.</value>
        IEvent OnLocationChanged { get; }
    }

    /// <summary>
    /// Allows changing the position of an entity.
    /// </summary>
    public interface ITranslateComponent : ITranslate, IComponent
    {        
    }
}
