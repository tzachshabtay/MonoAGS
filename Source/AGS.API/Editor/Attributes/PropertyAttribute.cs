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

        /// <summary>
        /// Gets or sets the category's z index for sorting in the inspector (this should match between all the properties in the category for accurate results).
        /// </summary>
        /// <value>The category z.</value>
        public int CategoryZ { get; set; }

        /// <summary>
        /// Gets or sets whether to expand the category automatically when opening the inspector (this should match between all the properties in the category for accurate results).
        /// </summary>
        /// <value>Expand Category?</value>
        public bool CategoryExpand { get; set; }

        /// <summary>
        /// Forces the property to be shown as readonly in the inspector (even if it can be set by script).
        /// </summary>
        /// <value><c>true</c> to force readonly; otherwise, <c>false</c>.</value>
        public bool ForceReadonly { get; set; }
    }
}
