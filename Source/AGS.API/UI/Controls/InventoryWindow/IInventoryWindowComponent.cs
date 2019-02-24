namespace AGS.API
{
    /// <summary>
    /// An inventory window allows displaying inventory items in a window.
    /// </summary>
    /// <seealso cref="IInventory"/>
    /// <seealso cref="IInventoryItem"/>
	[RequiredComponent(typeof(IScaleComponent))]
	[RequiredComponent(typeof(IInObjectTreeComponent))]
	public interface IInventoryWindowComponent : IComponent
	{
        /// <summary>
        /// Gets or sets the size which is allocated to each inventory item in the window.
        /// </summary>
        /// <value>The size of the item.</value>
		SizeF ItemSize { get; set; }

        /// <summary>
        /// Gets or sets the inventory which is displayed in the window.
        /// </summary>
        /// <value>The inventory.</value>
        IInventory Inventory { get; set; }

        /// <summary>
        /// Gets or sets the top item index for the current view of the window.
        /// If the top item is 0, the window will show starting from the first item,
        /// and up to the number of items that fit in the window.
        /// If the top item is 1, the window will skip the first item, and so on.
        /// </summary>
        /// <value>The top item.</value>
		int TopItem { get; set; }

        /// <summary>
        /// Optional padding from the left side of the inventory window containing panel.
        /// </summary>
        /// <value>The left padding.</value>
        float PaddingLeft { get; set; }

        /// <summary>
        /// Optional padding from the right side of the inventory window containing panel.
        /// </summary>
        /// <value>The right padding.</value>
        float PaddingRight { get; set; }

        /// <summary>
        /// Optional padding from the bottom side of the inventory window containing panel.
        /// </summary>
        /// <value>The bottom padding.</value>
        float PaddingBottom { get; set; }

        /// <summary>
        /// Optional padding from the top side of the inventory window containing panel.
        /// </summary>
        /// <value>The top padding.</value>
        float PaddingTop { get; set; }

        /// <summary>
        /// Optional padding to apply between the inventory items.
        /// </summary>
        /// <value>The padding between the inventory items.</value>
        SizeF PaddingBetweenItems { get; set; }

        /// <summary>
        /// Scrolls the inventory window up one row if possible (<see cref="TopItem"/> will change accordingly).
        /// </summary>
		void ScrollUp();

        /// <summary>
        /// Scrolls the inventory window down one row if possible (<see cref="TopItem"/> will change accordingly)  
        /// </summary>
		void ScrollDown();

        /// <summary>
        /// Gets the number of items that fit in one row.
        /// </summary>
        /// <value>The items per row.</value>
		int ItemsPerRow { get; }

        /// <summary>
        /// Gets the number of rows the inventory currently needs to be fully displayed.
        /// </summary>
        /// <value>The row count.</value>
		int RowCount { get; }

	}
}

