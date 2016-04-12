using System;

namespace AGS.API
{
	[AttributeUsage(AttributeTargets.Interface, AllowMultiple = true)]
	public class RequiredComponentAttribute : Attribute
	{
		public RequiredComponentAttribute(Type component, bool mandatory = true)
		{
			Component = component;
			Mandatory = true;
		}

		public Type Component { get; private set; }

		public bool Mandatory { get; private set; }
	}
}

