using System;
namespace AGS.API
{
    /// <summary>
    /// Allows attaching an attribute to a property which controls various aspects on how it will
    /// look when shown via the inspector.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class PropertyAttribute : Attribute
    {
        public PropertyAttribute()
        {
            Browsable = true;
        }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="T:AGS.API.PropertyAttribute"/> will be
        /// visible in the inspector.
        /// </summary>
        /// <value><c>true</c> if browsable; otherwise, <c>false</c>.</value>
        public bool Browsable { get; set; }

        /// <summary>
        /// Gets or sets the display name (this will override the property name if set).
        /// </summary>
        /// <value>The display name.</value>
        public string DisplayName { get; set; }

        /// <summary>
        /// Gets or sets the description (this will override the xml docs on the property if set).
        /// </summary>
        /// <value>The description.</value>
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets the category (this will override the component name, if part of a component, or the "General" category for all other properties).
        /// </summary>
        /// <value>The category.</value>
        public string Category { get; set; }
    }
}
