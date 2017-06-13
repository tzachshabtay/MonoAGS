using System;
using System.Collections.Generic;

namespace AGS.API
{
    /// <summary>
    /// The combo box component allows having the entity behave like a drop-down for selecting an item from a list.
    /// </summary>
    [RequiredComponent(typeof(IInObjectTree))]    
    public interface IComboBoxComponent : IComponent
    {
        /// <summary>
        /// Gets the selected item.
        /// </summary>
        /// <value>The selected item.</value>
        object SelectedItem { get; }

        /// <summary>
        /// Gets or sets the selected item index (a 0 based index of the items list).
        /// </summary>
        /// <value>The index of the selected.</value>
        int SelectedIndex { get; set; }

        /// <summary>
        /// Gets the list of items which are up for selection.
        /// </summary>
        /// <value>The items.</value>
        IList<object> Items { get; }

        /// <summary>
        /// Gets the drop down panel, which hosts the selection textbox and the dropdown button.
        /// </summary>
        /// <value>The drop down panel.</value>
        IPanel DropDownPanel { get; }

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
        /// Gets or sets the factory function for creating buttons which will be displayed for each item
        /// in the dropdown list.
        /// </summary>
        /// <value>The item button factory.</value>
        Func<IButton> ItemButtonFactory { get; set; }

        /// <summary>
        /// Gets the dropdown list item buttons.
        /// </summary>
        /// <value>The item buttons.</value>
        IEnumerable<IButton> ItemButtons { get; }

        /// <summary>
        /// An event which fires whenever the selected item in the dropdown was changed
        /// (this can be because the user selected an item, or because the <see cref="SelectedIndex"/> was set from code). 
        /// </summary>
        /// <value>The on selected item changed event.</value>
        IEvent<ComboboxItemArgs> OnSelectedItemChanged { get; }
    }
}
