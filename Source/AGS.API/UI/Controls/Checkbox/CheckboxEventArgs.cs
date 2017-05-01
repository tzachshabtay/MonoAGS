namespace AGS.API
{
    /// <summary>
    /// Event arguments for a checkbox check change.
    /// </summary>
    public class CheckBoxEventArgs : AGSEventArgs
    {
        public CheckBoxEventArgs(bool isChecked)
        {
            Checked = isChecked;
        }

        /// <summary>
        /// Gets a value indicating whether the checkbox is checked.
        /// </summary>
        /// <value><c>true</c> if checked; otherwise, <c>false</c>.</value>
        public bool Checked { get; private set; }
    }
}
