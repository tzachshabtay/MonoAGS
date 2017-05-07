using System;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace AGS.API
{
    /// <summary>
    /// Represents a sequence of actions that can be performed after a dialog choice.
    /// </summary>
	public interface IDialogActions
	{
        /// <summary>
        /// Gets the actions.
        /// </summary>
        /// <value>The actions.</value>
		IList<IDialogAction> Actions { get; }

        /// <summary>
        /// Adds an action for saying text by a character.
        /// </summary>
        /// <param name="character">The speaking character.</param>
        /// <param name="sentences">The list of sentences that the character will say one by one.</param>
		void AddText(ICharacter character, params string[] sentences);

        /// <summary>
        /// Adds an action for saying text by the player.
        /// </summary>
        /// <param name="sentences">The list of sentences that the player will say one by one.</param>
		void AddPlayerText(params string[] sentences);

        /// <summary>
        /// Adds a list of actions
        /// </summary>
        /// <param name="actions">Actions.</param>
		void AddActions(params Action[] actions);

        /// <summary>
        /// Adds a list of actions that may cancel the sequence.
        /// </summary>
        /// <param name="actions">Actions.</param>
		void AddConditionalActions(params Func<bool>[] actions);

        /// <summary>
        /// Adds a list of asynchronous actions.
        /// </summary>
        /// <param name="actions">Actions.</param>
		void AddAsyncActions(params Func<Task>[] actions);

        /// <summary>
        /// Adds a list of asynchronous actions that may cancel the sequence.
        /// </summary>
        /// <param name="actions">Actions.</param>
		void AddAsyncConditionalActions(params Func<Task<bool>>[] actions);

        /// <summary>
        /// Adds a list of dialog actions.
        /// </summary>
        /// <param name="actions">Actions.</param>
		void AddActions(params IDialogAction[] actions);

        /// <summary>
        /// Runs the actions list asynchronously, one by one.
        /// </summary>
        /// <returns>True if all the actions were executed, false if the sequence was cancelled.</returns>
		Task<bool> RunAsync();
	}
}

