using System;
using System.Collections.Generic;
using System.Text;

namespace AGS.API
{
    /// <summary>
    /// Allows to set the default fonts to be used in the game.
    /// </summary>
    public interface IDefaultFonts
    {
        /// <summary>
        /// Gets or sets the default speech font (for text displayed when characters are speaking if no other font was given).
        /// </summary>
        /// <value>The speech font.</value>
        IFont Speech { get; set; }

        /// <summary>
        /// Gets or sets the default text font (for any non-speech related text shown on screen if no other font was given).
        /// </summary>
        /// <value>The text font.</value>
        IFont Text { get; set; }

        /// <summary>
        /// Gets or sets the default dialog font (for rendering the dialog options). Leave as null to use the default text font as the default dialog font.
        /// </summary>
        IFont Dialogs { get; set; }
    }
}
