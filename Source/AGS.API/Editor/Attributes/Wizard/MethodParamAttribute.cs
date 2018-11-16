using System;
namespace AGS.API
{
    /// <summary>
    /// An attribute to configure parameters for a method wizard (<see cref="MethodWizardAttribute"/>).
    /// </summary>
    [AttributeUsage(AttributeTargets.Parameter)]
	public class MethodParamAttribute : Attribute
    {
        public MethodParamAttribute()
        {
            Browsable = true;
        }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="T:AGS.API.MethodParamAttribute"/> is visible
        /// in the wizard.
        /// </summary>
        /// <value><c>true</c> if browsable; otherwise, <c>false</c>.</value>
        public bool Browsable { get; set; }

        /// <summary>
        /// Gets or sets the default value for the parameter in the wizard.
        /// This is ignored if <see cref="DefaultProvider"/> is assigned.
        /// If not set, and the method itself has a default value, then that value will be used as default.
        /// </summary>
        /// <value>The default.</value>
        public object Default { get; set; }

        /// <summary>
        /// Gets or sets the default provider: this should be the name of a static method in the same class,
        /// which should return the default value. The method should accept a "Resolver" as an input parameter.
        /// </summary>
        /// <value>The default provider.</value>
        public string DefaultProvider { get; set; }
    }
}