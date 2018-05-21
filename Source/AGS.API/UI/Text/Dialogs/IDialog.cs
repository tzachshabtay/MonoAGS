using System.Collections.Generic;
using System.Threading.Tasks;

namespace AGS.API
{
    /// <summary>
    /// Represents a dialog (an on-going conversation) between two (or more) characters.
    /// The dialog is structured as a sequence of actions (usually the characters talking),
    /// followed by a list of choices for the player, where each choice branches to another sequence of actions,
    /// followed by another list of choices, and so on.
    /// </summary>
    public interface IDialog
	{
        /// <summary>
        /// The background graphics for the dialog.
        /// </summary>
        /// <value>The graphics.</value>
		IObject Graphics { get; }

        /// <summary>
        /// Gets the list of options for the user to choose from.
        /// </summary>
        /// <value>The options.</value>
		IList<IDialogOption> Options { get; }

        /// <summary>
        /// Gets a value indicating whether this <see cref="T:AGS.API.IDialog"/> should show while 
        /// the actions triggered by the selected option are running.
        /// </summary>
        /// <value><c>true</c> if show while options are running; otherwise, <c>false</c>.</value>
		bool ShowWhileOptionsAreRunning { get; }

        /// <summary>
        /// Gets the startup actions (will be performed before the dialog options are shown to the user).
        /// </summary>
        /// <value>The startup actions.</value>
		IDialogActions StartupActions { get; }

        /// <summary>
        /// Adds the specified options to the user's list of choices.
        /// </summary>
        /// <param name="options">Options.</param>
		void AddOptions(params IDialogOption[] options);

        /// <summary>
        /// Run the dialog asynchronously. This will run the <see cref="StartupActions"/>, followed by showing the
        /// <see cref="Options"/>  
        /// </summary>
		Task RunAsync();
	}
}

