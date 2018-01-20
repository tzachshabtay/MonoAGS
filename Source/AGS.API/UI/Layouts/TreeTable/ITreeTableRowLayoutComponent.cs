using System;
namespace AGS.API
{
    /// <summary>
    /// Represents a row in a tree view that aligns its columns with other rows, based on <see cref="ITreeTableLayout"/> .
    /// </summary>
    public interface ITreeTableRowLayoutComponent : IComponent
    {
        /// <summary>
        /// Gets or sets the table layout.
        /// </summary>
        /// <value>The table layout.</value>
        ITreeTableLayout Table { get; set; }
    }
}
