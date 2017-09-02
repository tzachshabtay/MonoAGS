namespace AGS.API
{
    /// <summary>
    /// Responsible for returning the current object that is located under the mouse position.
    /// </summary>
    public interface IHitTest
    {
        /// <summary>
        /// Gets the object at the mouse position (this ignores objects that are not visible or enabled).
        /// </summary>
        /// <value>The object at mouse position.</value>
        IObject ObjectAtMousePosition { get; }
    }
}
