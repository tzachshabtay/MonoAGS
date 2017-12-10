using System.Collections.Generic;

namespace AGS.API
{
    /// <summary>
    /// Custom properties that you can attach to entities and can be used to get/set values during the game.
    /// </summary>
    public interface ICustomPropertiesPerType<TValue>
    {
        /// <summary>
        /// Gets the stored value for the custom property with the specified name.
        /// If no value is stored for this property, returns the specified default value instead
        /// (and stored that default value as the property's new value).
        /// </summary>
        /// <returns>The value.</returns>
        /// <param name="name">Name.</param>
        /// <param name="defaultValue">Default value.</param>
        TValue GetValue(string name, TValue defaultValue = default);

        /// <summary>
        /// Stores a new value for the custom property with the specified name.
        /// If an old value is stored it will be overridden by the new value.
        /// </summary>
        /// <param name="name">Name.</param>
        /// <param name="value">Value.</param>
        void SetValue(string name, TValue value);

        /// <summary>
        /// Returns a dictionary with all existing custom properties for this type.
        /// </summary>
        /// <returns>The properties.</returns>
        IDictionary<string, TValue> AllProperties();

        /// <summary>
        /// Copies the custom properties from a different object (this will override
        /// values in the current object, but will not delete properties which don't
        /// have an equivalent in the given object).
        /// </summary>
        /// <param name="properties">Properties.</param>
        void CopyFrom(ICustomPropertiesPerType<TValue> properties);
    }
}
