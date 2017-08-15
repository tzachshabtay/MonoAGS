using System;
using System.Collections.Generic;

namespace AGS.API
{
    /// <summary>
    /// A component for displaying a list of items (as text), and allowing selecting an item from the list.
    /// </summary>
    [RequiredComponent(typeof(IScaleComponent))]
    [RequiredComponent(typeof(IInObjectTree))]
    [RequiredComponent(typeof(IImageComponent))]
    [RequiredComponent(typeof(IStackLayoutComponent))]
    public interface IListboxComponent : IComponent
    {
        /// <summary>
        /// Gets the selected item.
        /// </summary>
        /// <value>The selected item.</value>
        IStringItem SelectedItem { get; }

        /// <summary>
        /// Gets or sets the selected item index (a 0 based index of the items list).
        /// </summary>
        /// <value>The index of the selected.</value>
        int SelectedIndex { get; set; }

        /// <summary>
        /// Gets the list of items which are up for selection.
        /// </summary>
        /// <value>The items.</value>
        IAGSBindingList<IStringItem> Items { get; }

        /// <summary>
        /// Gets or sets the factory function for creating buttons which will be displayed for each item
        /// in the dropdown list. The function gets the text of the button (useful for giving a button id) and should return a button.
        /// </summary>
        /// <value>The item button factory.</value>
        Func<string, IButton> ItemButtonFactory { get; set; }

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
        IEvent<ListboxItemArgs> OnSelectedItemChanged { get; }
    }
}
