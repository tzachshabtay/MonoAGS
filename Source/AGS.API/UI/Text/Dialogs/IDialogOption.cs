using System;

namespace AGS.API
{
    /// <summary>
    /// Represents a dialog option that is shown on the screen as part of a list of options,
    /// where the user needs to choose one of the options.
    /// </summary>
    public interface IDialogOption : IDialogActions, IDisposable
	{
        /// <summary>
        /// Gets the label which will host the text for the dialog option.
        /// </summary>
        /// <value>The label.</value>
		ILabel Label { get; }

        /// <summary>
        /// Gets the text configuration (font, color, outline, etc) for when the mouse hovers over the option.
        /// By default it will color the text yellow.
        /// </summary>
        /// <value>The hover config.</value>
		ITextConfig HoverConfig { get; }

        /// <summary>
        /// Gets the text configuration (font, color, outline, etc) for when the option was already previously
        /// selected by the player.
        /// By default it will color the text gray.
        /// Note: if <see cref="ShowOnce"/> is false, then the option will be hidden and this configuration will
        /// be irrelevant. 
        /// </summary>
        /// <value>The "has been chosen" config.</value>
        ITextConfig HasBeenChosenConfig { get; }

        /// <summary>
        /// Gets a value indicating whether this <see cref="T:AGS.API.IDialogOption"/> should be spoken by
        /// the player before moving on to the list of actions.
        /// </summary>
        /// <value><c>true</c> if speak option; otherwise, <c>false</c>.</value>
		bool SpeakOption { get; }

        /// <summary>
        /// Gets a value indicating whether this <see cref="T:AGS.API.IDialogOption"/> should be shown after
        /// it was already selected once.
        /// Note: an alternative to this would be using <see cref="HasBeenChosenConfig"/>, which by default
        /// colors an already selected text gray.
        /// </summary>
        /// <value><c>true</c> if show once; otherwise, <c>false</c>.</value>
		bool ShowOnce { get; }

        /// <summary>
        /// Gets or sets a value indicating whether the dialog should close after this dialog option's actions
        /// are executed.
        /// </summary>
        /// <value><c>true</c> if exit dialog when finished; otherwise, <c>false</c>.</value>
		bool ExitDialogWhenFinished { get; set; }

        /// <summary>
        /// Gets or sets another dialog that will be changed to after this dialog option's actions are executed.
        /// </summary>
        /// <value>The change dialog when finished.</value>
		IDialog ChangeDialogWhenFinished { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="T:AGS.API.IDialogOption"/> has already been chosen
        /// by the user before.
        /// </summary>
        /// <value><c>true</c> if has option been chosen; otherwise, <c>false</c>.</value>
        bool HasOptionBeenChosen { get; set; }
	}
}

