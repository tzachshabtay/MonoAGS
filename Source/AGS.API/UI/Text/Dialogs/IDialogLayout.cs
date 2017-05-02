using System.Collections.Generic;
using System.Threading.Tasks;

namespace AGS.API
{
    /// <summary>
    /// Allows customizing how the dialog options will be layed out on the screen.
    /// The default will show them at the bottom of the screen.
    /// </summary>
    public interface IDialogLayout
	{
        /// <summary>
        /// Asynchronously lays out the dialog options on the screen..
        /// </summary>
        /// <returns>The task to await.</returns>
        /// <param name="dialogGraphics">The dialog background graphics.</param>
        /// <param name="options">The list of dialog options to show.</param>
		Task LayoutAsync(IObject dialogGraphics, IList<IDialogOption> options);
	}
}

