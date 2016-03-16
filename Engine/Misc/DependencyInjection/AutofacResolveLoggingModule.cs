using System;
using Autofac;
using Autofac.Core;
using System.Diagnostics;

namespace AGS.Engine
{
	/// <summary>
	/// Used to debug dependency resolve issues.
	/// Source: http://stackoverflow.com/questions/18578942/how-can-i-log-all-resolve-requests-to-autofac-container
	/// </summary>
	public class AutofacResolveLoggingModule : Module
	{
		public int depth = 0;

		protected override void AttachToComponentRegistration(IComponentRegistry componentRegistry,
			IComponentRegistration registration)
		{
			registration.Preparing += RegistrationOnPreparing;
			registration.Activating += RegistrationOnActivating;
			base.AttachToComponentRegistration(componentRegistry, registration);
		}

		private string GetPrefix()
		{
			return new string('-',  depth * 2);
		}

		private void RegistrationOnPreparing(object sender, PreparingEventArgs preparingEventArgs)
		{
			Debug.WriteLine("{0}Resolving  {1}", GetPrefix(), preparingEventArgs.Component.Activator.LimitType);
			depth++;
		}

		private void RegistrationOnActivating(object sender, ActivatingEventArgs<object> activatingEventArgs)
		{
			depth--;    
			Debug.WriteLine("{0}Activating {1}", GetPrefix(), activatingEventArgs.Component.Activator.LimitType);
		}
	}
}

