using System;
namespace AGS.API
{
    /// <summary>
    /// This attribute can be assigned to concrete implementations of interfaces.
    /// This affects the inspector, when selecting implementations for interfaces (for example, when selecting a border you'd want to look of implmentations of <see cref="IBorderStyle"/>).
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class ConcreteImplementationAttribute : Attribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="T:AGS.API.ConcreteImplementationAttribute"/> class.
        /// </summary>
        public ConcreteImplementationAttribute()
        {
            Browsable = true;
        }

        /// <summary>
        /// Gets or sets a value indicating whether the associated class can be selected as a possible implementation.
        /// </summary>
        /// <value><c>true</c> if browsable; otherwise, <c>false</c>.</value>
        public bool Browsable { get; set; }

        /// <summary>
        /// Gets or sets the display name to be shown instead of the class name.
        /// </summary>
        /// <value>The display name.</value>
        public string DisplayName { get; set; }
    }
}