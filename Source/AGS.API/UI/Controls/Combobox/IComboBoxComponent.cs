using System;

namespace AGS.API
{
    /// <summary>
    /// Which mode to apply when typing text in the combo box's textbox?
    /// </summary>
    public enum ComboSuggest
    {
        /// <summary>
        /// The textbox is disabled, combobox selection must be made with the drop-down alone (best for comboboxes with a small number of options).
        /// </summary>
        None,
        /// <summary>
        /// User gets suggestions when typing in the textbox (based on the combobox items), however those are only suggestions, and the user is free
        /// to type text which is not part of the combobox options. An example where this can be used is in a color selector, where you have a list of available colors (in the dropdown)
        /// but also the ability to type RGB values directly.
        /// </summary>
        Suggest,
        /// <summary>
        /// User can type in the textbox to filter the options in the combobox, but the user does not have the ability to type text which is not part of the combobox options (best for comboboxes with a lot of options).
        /// </summary>
        Enforce,
    }

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

        /// <summary>
        /// Gets or sets the suggestion mode (how does the combobox behave when the user types text directly in the textbox instead of using the dropdown button).
        /// </summary>
        /// <value>The suggestion mode.</value>
        ComboSuggest SuggestMode { get; set; }

        /// <summary>
        /// Allows to override the behavior when marking suggestions on the dropdown as the user types text in the textbox.
        /// By default, the suggestion is given an orange color, but if you want a different behavior, set this callback which gets 2 parameters: 
        /// the old button which is no longer the current marked suggestion (passed to clear the current mark) and the new button which is the new suggestion
        /// in the dropdown (passed to "draw" the mark on the button).
        /// </summary>
        /// <value>The mark suggestion.</value>
        Action<IButton, IButton> MarkComboboxSuggestion { get; set; }
    }
}
