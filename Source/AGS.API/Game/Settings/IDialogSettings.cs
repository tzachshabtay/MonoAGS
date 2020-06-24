namespace AGS.API
{
    /// <summary>
    /// Allows to set default settings to control how dialogs look and behave.
    /// </summary>
    public interface IDialogSettings
    {
        /// <summary>
        /// The default <see cref="IRenderLayer"/> used to display the dialogs (by default, this will be AGSLayers.Dialog).
        /// </summary>
        IRenderLayer RenderLayer { get; set; }

        /// <summary>
        /// The default text configurations for showing dialog options when they're in their "normal" state.
        /// </summary>
        ITextConfig Idle { get; set; }

        /// <summary>
        /// The default text configurations for showing dialog options when they're hovered on via the mouse.
        /// </summary>
        ITextConfig Hovered { get; set; }

        /// <summary>
        /// The default text configurations for showing dialog options after they have already been used before.
        /// </summary>
        ITextConfig Chosen { get; set; }
    }
}
