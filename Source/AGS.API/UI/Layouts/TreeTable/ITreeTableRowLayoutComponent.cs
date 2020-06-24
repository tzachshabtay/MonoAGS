using System.Collections.Generic;

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

        /// <summary>
        /// Allows disabling specific entities from changing the overall aligned columns.
        /// </summary>
        /// <value>The restriction list.</value>
        IRestrictionList RestrictionList { get; }

        /// <summary>
        /// Allow overriding the width for specific columns.
        /// The key is the index of the column (zero based, so 0 is the first column) and the value is the width.
        /// </summary>
        /// <value>The fixed width overrides.</value>
        Dictionary<int, float> FixedWidthOverrides { get; }
    }
}