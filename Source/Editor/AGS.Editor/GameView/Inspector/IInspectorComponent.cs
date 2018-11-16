using AGS.API;

namespace AGS.Editor
{
    /// <summary>
    /// Allows to "inspect" objects to show (and allow editing) all of the object's properties in a GUI.
    /// </summary>
    public interface IInspectorComponent : IComponent
    {
        /// <summary>
        /// Show the specified object's properties in a GUI.
        /// </summary>
        /// <returns>True if there as anything to show, otherwise (if there were no properties to show) false.</returns>
        /// <param name="obj">Object.</param>
        bool Show(object obj);
    }
}
