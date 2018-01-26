using System.Collections.Generic;

namespace AGS.API
{
    /// <summary>
    /// Event arguments used by <see cref="ITreeTableLayout"/> when querying the rows for the layout.
    /// Each row should set a new column size for each column with a size smaller than what the row needs.
    /// </summary>
    public class QueryLayoutEventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="T:AGS.API.QueryLayoutEventArgs"/> class.
        /// </summary>
        public QueryLayoutEventArgs()
        {
            ColumnSizes = new List<float>(10);
        }

        /// <summary>
        /// Gets the column sizes.
        /// Each row should set a new column size for each column with a size smaller than what the row needs.
        /// </summary>
        /// <value>The column sizes.</value>
        public List<float> ColumnSizes { get; private set;}
    }
}
