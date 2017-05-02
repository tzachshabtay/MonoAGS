using System.Threading.Tasks;

namespace AGS.API
{
    /// <summary>
    /// A dialog action.
    /// This is usually text spoken by one of the characters, but it can be any action,
    /// which will run in the sequence.
    /// </summary>
    public interface IDialogAction
	{
        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="T:AGS.API.IDialogAction"/> is enabled.
        /// If it's disabled it will be skipped.
        /// </summary>
        /// <value><c>true</c> if enabled; otherwise, <c>false</c>.</value>
		bool Enabled { get; set; }

        /// <summary>
        /// Runs the dialog action asynchronously.
        /// </summary>
        /// <returns>True if should continue to the next action, false if should cancel the sequence of actions.</returns>
		Task<bool> RunActionAsync();
	}
}

