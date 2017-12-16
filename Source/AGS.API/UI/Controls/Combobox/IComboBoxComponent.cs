namespace AGS.API
{
    /// <summary>
    /// The combo box component allows having the entity behave like a drop-down for selecting an item from a list.
    /// </summary>
    public interface IComboBoxComponent : IComponent
    {
        /// <summary>
        /// Gets or sets the text box which shows the display text of the selected item.
        /// </summary>
        /// <value>The text box.</value>
        ITextBox TextBox { get; set; }

        /// <summary>
        /// Gets or sets the drop down button, which shows the drop down list when clicked.
        /// </summary>
        /// <value>The drop down button.</value>
        IButton DropDownButton { get; set; }

        /// <summary>
        /// Sets the drop down panel which hosts the selection item.
        /// The entity is expected to have the following components:
        /// <see cref="IListboxComponent"/> , <see cref="IVisibleComponent"/> , <see cref="IDrawableInfoComponent"/>,
        /// <see cref="IImageComponent"/> , <see cref="ITranslateComponent"/>   
        /// </summary>
        IEntity DropDownPanel { get; set; }

        /// <summary>
        /// Gets the drop down panel list of items.
        /// </summary>
        /// <value>The drop down panel.</value>
        IListboxComponent DropDownPanelList { get; }
    }
}
