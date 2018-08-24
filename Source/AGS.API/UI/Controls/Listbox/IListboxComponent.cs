using System;
using System.Collections.Generic;

namespace AGS.API
{
    /// <summary>
    /// A component for displaying a list of items (as text), and allowing selecting an item from the list.
    /// </summary>
    [RequiredComponent(typeof(IScaleComponent))]
    [RequiredComponent(typeof(IInObjectTreeComponent))]
    [RequiredComponent(typeof(IImageComponent))]
    [RequiredComponent(typeof(IStackLayoutComponent))]
    [RequiredComponent(typeof(IVisibleComponent))]
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
        /// Gets or sets the factory function for creating the UI controls which will be displayed for each item
        /// in the list. The function gets the text of the item and should return a UI control.
        /// </summary>
        /// <value>The list item factory.</value>
        Func<string, IUIControl> ListItemFactory { get; set; }

        /// <summary>
        /// Gets the list item UI controls.
        /// </summary>
        /// <value>The item UI controls.</value>
        IEnumerable<IUIControl> ListItemUIControls { get; }

        /// <summary>
        /// An event which fires whenever the selected item in the dropdown was changed
        /// (this can be because the user selected an item, or because the <see cref="SelectedIndex"/> was set from code). 
        /// </summary>
        /// <value>The on selected item changed event.</value>
        IBlockingEvent<ListboxItemArgs> OnSelectedItemChanged { get; }

        /// <summary>
        /// An event which fires whenever the selected item in the dropdown is in the process of being changed
        /// (this can be because the user selected an item, or because the <see cref="SelectedIndex"/> was set from code).
        /// The selection has not been applied yet in this event, and can be cancelled by the event subscriber by settings <see cref="ListboxItemChangingArgs.ShouldCancel"/> to true.
        /// </summary>
        /// <value>The on selected item changing event.</value>
        IEvent<ListboxItemChangingArgs> OnSelectedItemChanging { get; }

        /// <summary>
        /// Gets or sets the minimum height for the box.
        /// </summary>
        /// <value>The minimum height.</value>
        float MinHeight { get; set; }

        /// <summary>
        /// Gets or sets the maximum height of the box.
        /// If it is expected for the items in the list to exceed the height, scrollbars 
        /// should be added to the list (<see cref="IUIFactory.CreateScrollingPanel"/> ).
        /// </summary>
        /// <value>The height of the max.</value>
        float MaxHeight { get; set; }

        /// <summary>
        /// Gets or sets the search filter (a search text that filters the tree so that
        /// only items containing the text appear in the list).
        /// </summary>
        /// <value>The search filter.</value>
        string SearchFilter { get; set; }
    }
}
