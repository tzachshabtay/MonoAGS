namespace AGS.API
{
    /// <summary>
    /// A form (a panel with a header).
    /// </summary>
    public interface IForm
    {
        /// <summary>
        /// Gets the header.
        /// </summary>
        /// <value>The header.</value>
        ILabel Header { get; }

        /// <summary>
        /// Gets the contents panel.
        /// </summary>
        /// <value>The contents.</value>
        IPanel Contents { get; }

        /// <summary>
        /// Gets or sets the width.
        /// </summary>
        /// <value>The width.</value>
        float Width { get; set; }

        /// <summary>
        /// Gets the height.
        /// </summary>
        /// <value>The height.</value>
        float Height { get; }

        /// <summary>
        /// Gets or sets the x.
        /// </summary>
        /// <value>The x.</value>
        float X { get; set; }

        /// <summary>
        /// Gets or sets the y.
        /// </summary>
        /// <value>The y.</value>
        float Y { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="T:AGS.API.IForm"/> is visible.
        /// </summary>
        /// <value><c>true</c> if visible; otherwise, <c>false</c>.</value>
        bool Visible { get; set; }
    }
}