namespace AGS.API
{
    /// <summary>
    /// An entity is the most basic unit for composition.
    /// On itself it doesn't do anything except be a collection of components.
    /// Each added component adds more abilities to the entity.
    /// For example, adding a "talk" component to the entity, will give the entity the ability to talk.
    /// </summary>
	public interface IEntity : IComponentsCollection
	{
        /// <summary>
        /// A unique identifier for the entity. 
        /// This id is used by the engine to distinguish between different entities, so 
        /// the id must be unique.
        /// </summary>
        /// <value>The identifier.</value>
		string ID { get; }

        /// <summary>
        /// A human-readable name.
        /// This string could be used to display entity's name to players.
        /// </summary>
        /// <value>The name.</value>
        string DisplayName { get; set; }

        /// <summary>
        /// Returns a human-readable name of an entity, either taken from DisplayName or composed from other properties.
        /// </summary>
        /// <returns>The name.</returns>
        string GetFriendlyName();
    }
}

