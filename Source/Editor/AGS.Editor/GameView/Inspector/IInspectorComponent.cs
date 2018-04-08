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
        /// <returns>The show.</returns>
        /// <param name="obj">Object.</param>
        void Show(object obj);
    }
}
