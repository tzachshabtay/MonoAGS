namespace AGS.API
{
    /// <summary>
    /// An icon which has two modes: selected and not selected.
    /// </summary>
    public interface ISelectableIcon : IBorderStyle
    {
        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="T:AGS.API.ISelectableIcon"/> is selected.
        /// </summary>
        /// <value><c>true</c> if is selected; otherwise, <c>false</c>.</value>
        bool IsSelected { get; set; }
    }
}
