using System;
namespace AGS.API
{
    /// <summary>
    /// Gets or sets settings for various defaults.
    /// </summary>
    public interface IDefaultsSettings
    {
        /// <summary>
        /// Gets or sets the default speech font (for text displayed when characters are speaking if no other font was given).
        /// </summary>
        /// <value>The speech font.</value>
        IFont SpeechFont { get; set; }

        /// <summary>
        /// Gets or sets the default text font (for any non-speech related text shown on screen if no other font was given).
        /// </summary>
        /// <value>The text font.</value>
        IFont TextFont { get; set; }

        /// <summary>
        /// Gets or sets the default skin (which applies a consistent look to GUIs).
        /// </summary>
        /// <value>The skin.</value>
        ISkin Skin { get; set; }
    }
}