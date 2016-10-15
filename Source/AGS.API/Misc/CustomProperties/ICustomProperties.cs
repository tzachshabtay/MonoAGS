namespace AGS.API
{
    /// <summary>
    /// Custom properties that you can attach to entities and can be used to get/set values during the game.
    /// </summary>
    public interface ICustomProperties
    {
        /// <summary>
        /// Gets a collection of properties which type is an integer (whole numbers).
        /// </summary>
        /// <value>The ints.</value>
        ICustomPropertiesPerType<int> Ints { get; }

        /// <summary>
        /// Gets a collection of properties which type is a float (numbers with decimal point).
        /// </summary>
        /// <value>The floats.</value>
        ICustomPropertiesPerType<float> Floats { get; }

        /// <summary>
        /// Gets a collection of properties which type is a string (text).  
        /// </summary>
        /// <value>The strings.</value>
        ICustomPropertiesPerType<string> Strings { get; }

        /// <summary>
        /// Gets a collection of properties which type is a boolean (true/false).
        /// </summary>
        /// <value>The bools.</value>
        ICustomPropertiesPerType<bool> Bools { get; }

        /// <summary>
        /// Gets a collection of properties which type is an entity (can be any game object).
        /// </summary>
        /// <value>The entities.</value>
        ICustomPropertiesPerType<IEntity> Entities { get; }

        void RegisterCustomData(ICustomSerializable customData);

        /// <summary>
        /// Copies the custom properties from the specified object into this object.
        /// This will override existing values, but will not delete custom properties with
        /// no equivalent in the given object.
        /// </summary>
        /// <param name="properties">Properties.</param>
        void CopyFrom(ICustomProperties properties);
    }

    public interface ICustomPropertiesComponent : IComponent
    {
        /// <summary>
        /// Custom properties that are attached to the entity and can be used to get/set values during the game.
        /// </summary>
        ICustomProperties Properties { get; }
    }
}

