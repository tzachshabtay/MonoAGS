using System;
namespace AGS.API
{
    /// <summary>
    /// This attribute can be associated to an interface, to allow specifying factory methods that
    /// can be used to create a possible implementation. This is used by the inspector when selecting an implementation
    /// for an interface. For example, when selecting a border via <see cref="IBorderStyle"/>, it can refer you to various methods in <see cref="IBorderFactory"/>.
    /// </summary>
    [AttributeUsage(AttributeTargets.Interface, AllowMultiple = true)]
	public class HasFactoryAttribute : Attribute
    {
        /// <summary>
        /// Gets or sets the type of the factory.
        /// </summary>
        /// <value>The type of the factory.</value>
        public string FactoryType { get; set; }

        /// <summary>
        /// Gets or sets the name of the method in the factory object.
        /// </summary>
        /// <value>The name of the method.</value>
        public string MethodName { get; set; }
    }
}