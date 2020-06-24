using System;

namespace AGS.API
{
    /// <summary>
    /// A decorator for components, to hint that this component requires other components
    /// in order to function.
    /// For example, a walk component needs a translate (x,y,z) component to move x and y when the character
    /// moves. The walk component also needs an animation component to animate the walk.
    /// So a walk component will have a required component attribute with translate compnent, and another 
    /// attribute with the animation component.
    /// </summary>
    [AttributeUsage(AttributeTargets.Interface | AttributeTargets.Class, AllowMultiple = true)]
	public class RequiredComponentAttribute : Attribute
	{
		public RequiredComponentAttribute(Type component, bool mandatory = true)
		{
			Component = component;
			Mandatory = mandatory;
		}

        /// <summary>
        /// Gets the component.
        /// </summary>
        /// <value>The component.</value>
		public Type Component { get; private set; }

        /// <summary>
        /// Gets a value indicating whether the component is mandatory for the current component to function.
        /// </summary>
        /// <value><c>true</c> if mandatory; otherwise, <c>false</c>.</value>
		public bool Mandatory { get; private set; }
	}
}

