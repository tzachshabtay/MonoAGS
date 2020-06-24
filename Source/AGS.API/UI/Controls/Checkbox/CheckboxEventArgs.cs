namespace AGS.API
{
    /// <summary>
    /// Event arguments for a checkbox check change.
    /// </summary>
    public class CheckBoxEventArgs
    {
        public CheckBoxEventArgs(bool isChecked, bool userInitiated)
        {
            Checked = isChecked;
            UserInitiated = userInitiated;
        }

        /// <summary>
        /// Gets a value indicating whether the checkbox is checked.
        /// </summary>
        /// <value><c>true</c> if checked; otherwise, <c>false</c>.</value>
        public bool Checked { get; private set; }

        /// <summary>
        /// Gets a value indicating whether the checkbox was checked by the user or changed programmatically.
        /// </summary>
        /// <value><c>true</c> if user initiated; otherwise, <c>false</c>.</value>
        public bool UserInitiated { get; private set; }
    }
}
