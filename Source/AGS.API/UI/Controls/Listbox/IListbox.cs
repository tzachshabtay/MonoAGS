namespace AGS.API
{
    /// <summary>
    /// Represents a listbox (a GUI view of a list of items).
    /// </summary>
    public interface IListbox
    {
        /// <summary>
        /// The containing panel which carries the scroll bars.
        /// </summary>
        /// <value>The scrolling panel.</value>
        IPanel ScrollingPanel { get; }

        /// <summary>
        /// The panel containing the listbox items.
        /// </summary>
        /// <value>The contents panel.</value>
        IPanel ContentsPanel { get; }

        /// <summary>
        /// The listbox component.
        /// </summary>
        /// <value>The listbox component.</value>
        IListboxComponent ListboxComponent { get; }
    }
}