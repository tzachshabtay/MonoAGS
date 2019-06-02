namespace AGS.API
{
    /// <summary>
    /// Gets or sets settings for various defaults.
    /// </summary>
    public interface IDefaultsSettings
    {
        /// <summary>
        /// Gets or sets the default fonts to be used in the game.
        /// </summary>
        IDefaultFonts Fonts { get; set; }

        /// <summary>
        /// Gets or sets the default skin (which applies a consistent look to GUIs).
        /// </summary>
        /// <value>The skin.</value>
        ISkin Skin { get; set; }

        /// <summary>
        /// Gets or sets the default settings for showing message boxes (like Display, Yes/No prompts, etc).
        /// </summary>
        /// <value>The message box.</value>
        IMessageBoxSettings MessageBox { get; set; }

        /// <summary>
        /// Gets or sets default settings to control how dialogs look and behave.
        /// </summary>
        IDialogSettings Dialog { get; set; }
    }
}
