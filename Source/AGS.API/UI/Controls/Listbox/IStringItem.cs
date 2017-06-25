namespace AGS.API
{
    /// <summary>
    /// A textual item to be shown on GUI controls (like combobox, listbox or tree).
    /// </summary>
    public interface IStringItem
    {
        /// <summary>
        /// Gets or sets the text of the item.
        /// </summary>
        /// <value>The text.</value>
        string Text { get; set; }

        /// <summary>
        /// Gets or sets the text rendering configuration for the item.
        /// </summary>
        /// <value>The idle text config.</value>
        ITextConfig IdleTextConfig { get; set; }

        /// <summary>
        /// Gets or sets the text rendering configuration for the item when the mouse is hovered over it.
        /// </summary>
        /// <value>The hover text config.</value>
        ITextConfig HoverTextConfig { get; set; }

        /// <summary>
        /// Custom properties which can be assigned to the item and give more context.
        /// </summary>
        /// <value>The custom properties.</value>
        ICustomProperties Properties { get; }
    }
}
