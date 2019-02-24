using System;

namespace AGS.API
{
    /// <summary>
    /// Represents a layout of columns that is shared between nodes in a tree view, using a <see cref="ITreeTableRowLayoutComponent"/>.
    /// By sharing the interaces different nodes on the list can align the columns together.
    /// </summary>
    public interface ITreeTableLayout : IDisposable
    {
        /// <summary>
        /// Gets or sets the column padding.
        /// </summary>
        /// <value>The column padding.</value>
        float ColumnPadding { get; set; }

        /// <summary>
        /// Gets or sets the start x.
        /// </summary>
        /// <value>The start x.</value>
        float StartX { get; set; }

        /// <summary>
        /// Gets the column sizes.
        /// </summary>
        /// <value>The column sizes.</value>
        IAGSBindingList<float> ColumnSizes { get; }

        /// <summary>
        /// Gets the rows.
        /// </summary>
        /// <value>The rows.</value>
        IAGSBindingList<ITreeTableRowLayoutComponent> Rows { get; }

        /// <summary>
        /// Performs the layout.
        /// </summary>
        void PerformLayout();

        /// <summary>
        /// The table layout fires this event when it needs to query the column sizes from all the rows.
        /// The rows respond by setting their sizes, so at the end of the event, the layout knows the biggest
        /// length needed for each column.
        /// </summary>
        /// <value>The query layout event.</value>
        IBlockingEvent<QueryLayoutEventArgs> OnQueryLayout { get; }

        /// <summary>
        /// Once the table layout calculates new column sizes it fires this event, for which all the rows should
        /// listen and readjust their columns accordingly.
        /// If a specific row is passed, only that row needs to recalculate. If the passed row is null, all rows need to recalculate.
        /// </summary>
        /// <value>The refresh layout event.</value>
        IBlockingEvent<ITreeTableRowLayoutComponent> OnRefreshLayoutNeeded { get; }
    }
}
