namespace AGS.API
{
    /// <summary>
    /// Allows providing behavior for custom searching text in tree view or list box.
    /// </summary>
    public interface ICustomSearchItem
    {
        /// <summary>
        /// Is the search text contained in the item?
        /// </summary>
        /// <returns>The contains.</returns>
        /// <param name="searchText">Search text.</param>
        bool Contains(string searchText);
    }
}
