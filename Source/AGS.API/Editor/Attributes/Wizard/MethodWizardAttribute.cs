using System;
namespace AGS.API
{
    /// <summary>
    /// An attribute to mark methods that can act as wizards in the editor (can show a window to let the user set the parameters for the method).
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
	public class MethodWizardAttribute : Attribute
    {
    }
}
