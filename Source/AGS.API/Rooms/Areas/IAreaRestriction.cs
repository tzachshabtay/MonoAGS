using System.ComponentModel;

namespace AGS.API
{
    /// <summary>
    /// Defines how a restriction list (<see cref="IRestrictionList"/>) is interpreted.
    /// </summary>
    public enum RestrictionListType
    {
		/// <summary>
		/// Using a black list means that everybody on the list are restricted and will not be affected (by the area, viewport, or whatever the context of the list is),
		/// everybody else will be 'welcome' and will be affected (by the area, viewport, or whatever the context of the list is).
		/// </summary>
		BlackList,
		/// <summary>
		/// Using a white list means that everybody on the list are 'welcome' and will be affected  (by the area, viewport, or whatever the context of the list is),
		/// everybody else will be restricted and will not be affected  (by the area, viewport, or whatever the context of the list is).
		/// </summary>
		WhiteList,
    }

    /// <summary>
    /// Adds the ability to restrict certain entities from being affected by "something".
    /// That "something" can be an area, for example, or a display list for a viewport.
    /// </summary>
    public interface IRestrictionList : INotifyPropertyChanged
    {
        /// <summary>
        /// Gets or sets the type of the restriction, black list (everybody in the list is restricted)
        /// or white list (everybody not in the list is restricted).
        /// </summary>
        /// <value>The type of the restriction.</value>
        RestrictionListType RestrictionType { get; set; }

        /// <summary>
        /// Gets list of restricted entities (if it's a black list), or permitted entities (if it's a white list).
        /// If the list is empty then the component is effectively ignored (everybody is permitted).
        /// </summary>
        /// <value>The restriction list.</value>
        IConcurrentHashSet<string> RestrictionList { get; }

        /// <summary>
        /// Checks if an entity is restricted to this area (based on the restriction type and list).
        /// </summary>
        /// <returns><c>true</c>, if entity is restricted, <c>false</c> otherwise.</returns>
        /// <param name="entityId">Entity identifier.</param>
        bool IsRestricted(string entityId);
    }

    /// <summary>
    /// Adds the ability to restrict certain entities from being affected by the area.
    /// </summary>
    [RequiredComponent(typeof(IArea))]
    public interface IAreaRestriction : IRestrictionList, IComponent
    {
    }
}
