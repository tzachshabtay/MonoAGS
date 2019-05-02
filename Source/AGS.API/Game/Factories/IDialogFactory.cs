namespace AGS.API
{
    /// <summary>
    /// A factory to create dialogs and dialog options.
    /// </summary>
    public interface IDialogFactory
	{
        /// <summary>
        /// Creates a dialog option.
        /// </summary>
        /// <returns>The new dialog option.</returns>
        /// <param name="text">The text that appears in the dialog option.</param>
        /// <param name="config">Configuration for how to render the text.</param>
        /// <param name="hoverConfig">Configuration for how to render the text when the mouse is hovering it.</param>
        /// <param name="hasBeenChosenConfig">Configuration for how to render the text when it was already previously selected.
        /// If showOnce is true, the option will be hidden and this configuration is irrelevant.</param>
        /// <param name="speakOption">Should the character say the option after the user selects it?</param>
        /// <param name="showOnce">Should the engine hide the option after selecting it once?</param>
		IDialogOption GetDialogOption(string text, ITextConfig config = null, ITextConfig hoverConfig = null,
			ITextConfig hasBeenChosenConfig = null, bool speakOption = true, bool showOnce = false);

        /// <summary>
        /// Creates a dialog option.
        /// </summary>
        /// <returns>The new dialog option.</returns>
        /// <param name="text">The text that appears in the dialog option.</param>
        /// <param name="font">Font for rendering the text.</param>
        /// <param name="speakOption">Should the character say the option after the user selects it?</param>
        /// <param name="showOnce">Should the engine hide the option after selecting it once?</param>
        IDialogOption GetDialogOption(string text, IFont font, bool speakOption = true, bool showOnce = false);

        /// <summary>
        /// Creates a new dialog.
        /// </summary>
        /// <returns>The new dialog.</returns>
        /// <param name="id">A unique id for the dialog.</param>
        /// <param name="x">The x coordinate of the dialog left side.</param>
        /// <param name="y">The y coordinate of the dialog bottom side.</param>
        /// <param name="graphics">The background graphic object of the dialog.</param>
        /// <param name="showWhileOptionsAreRunning">Should the dialog be displayed while the characters are speaking?</param>
        /// <param name="options">The list of dialog options to display.</param>
		IDialog GetDialog(string id, float x = 0f, float y = 0f, IObject graphics = null, 
			bool showWhileOptionsAreRunning = false, params IDialogOption[] options);
	}
}

