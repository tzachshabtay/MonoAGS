using System;
namespace AGS.API
{
    /// <summary>
    /// Indicates when to apply a custom display string instead of using ToString().
    /// </summary>
    public enum CustomStringApplyWhen
    {
        /// <summary>
        /// Custom string will be displayed if the property is read-only.
        /// ToString() will still be used if the property can be written to.
        /// </summary>
        ReadOnly,
        /// <summary>
        /// Custom string will be displayed if the property can be written to.
        /// ToString() will still be used if the property is read-only.
        /// </summary>
        CanWrite,
        /// <summary>
        /// Custom string will always be displayed instead of ToString().
        /// </summary>
        Both
    }

    /// <summary>
    /// An attribute to be placed on a method that will be used instead of ToString() to supply the string display value
    /// to the inspector.
    /// The method must accept no arguments and return a string.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public class CustomStringValueAttribute : Attribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="T:AGS.API.CustomStringValueAttribute"/> class.
        /// </summary>
        /// <param name="applyWhen">Apply when.</param>
        public CustomStringValueAttribute(CustomStringApplyWhen applyWhen)
        {
            ApplyWhen = applyWhen;    
        }

        /// <summary>
        /// When to display the custom string instead of ToString()?
        /// </summary>
        public CustomStringApplyWhen ApplyWhen { get; private set; }
    }
}
