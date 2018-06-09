using System;
namespace AGS.API
{
    /// <summary>
    /// Event arguments for when the number editor's value changed.
    /// </summary>
    public class NumberValueChangedArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="T:AGS.API.NumberValueChangedArgs"/> class.
        /// </summary>
        /// <param name="userInitiated">If set to <c>true</c> user initiated.</param>
        public NumberValueChangedArgs(bool userInitiated)
        {
            UserInitiated = userInitiated;
        }

        /// <summary>
        /// Gets a value indicating whether the value was changed by the user (using the editor) or programmatically.
        /// </summary>
        /// <value><c>true</c> if user initiated; otherwise, <c>false</c>.</value>
        public bool UserInitiated { get; }
    }
}