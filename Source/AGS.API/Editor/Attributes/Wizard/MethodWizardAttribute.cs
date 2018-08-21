using System;
namespace AGS.API
{
    /// <summary>
    /// An attribute to mark methods that can act as wizards in the editor (can show a window to let the user set the parameters for the method).
    /// </summary>
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Constructor)]
	public class MethodWizardAttribute : Attribute
    {
        /// <summary>
        /// For a method returning multiple entities to be added to the scene, this should be a name of a public static method
        /// in the class which transforms the result of the method to a list of entities. The method should get an object (result) and return 
        /// a list of objects (the list of entities).
        /// </summary>
        /// <value>The entities provider.</value>
        public string EntitiesProvider { get; set; }
    }
}
