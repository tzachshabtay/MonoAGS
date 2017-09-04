using System.Threading.Tasks;

namespace AGS.API
{
    /// <summary>
    /// Allows loading a mask from an image.
    /// </summary>
    public interface IMaskLoader
	{
        /// <summary>
        /// Load the mask from the specified resource/file path (<see cref="IResourceLoader"/>).
        /// </summary>
        /// <returns>The mask.</returns>
        /// <param name="path">The resource/file path.</param>
        /// <param name="transparentMeansMasked">If set to <c>true</c> then a transparent pixel means a masked value, otherwise a non transparent pixel means a masked value.</param>
        /// <param name="debugDrawColor">For debugging purposes you can specify a color, which will then add an object to the mask which can drawn on screen (will use the color to draw the mask).</param>
        /// <param name="saveMaskToFile">Whether to save the mask to file (for debugging purposes).</param>
        /// <param name="id">A unique identifier for the mask (the path will be used as id if an id is not specified).</param>
		IMask Load(string path, bool transparentMeansMasked = false, Color? debugDrawColor = null, 
			string saveMaskToFile = null, string id = null);

        /// <summary>
        /// Load the mask asynchronously from the specified resource/file path (<see cref="IResourceLoader"/>).
        /// </summary>
        /// <returns>The mask.</returns>
        /// <param name="path">The resource/file path.</param>
        /// <param name="transparentMeansMasked">If set to <c>true</c> then a transparent pixel means a masked value, otherwise a non transparent pixel means a masked value.</param>
        /// <param name="debugDrawColor">For debugging purposes you can specify a color, which will then add an object to the mask which can drawn on screen (will use the color to draw the mask).</param>
        /// <param name="saveMaskToFile">Whether to save the mask to file (for debugging purposes).</param>
        /// <param name="id">A unique identifier for the mask (the path will be used as id if an id is not specified).</param>
		Task<IMask> LoadAsync (string path, bool transparentMeansMasked = false,
		    Color? debugDrawColor = null, string saveMaskToFile = null, string id = null);

        /// <summary>
        /// Load the mask from the specified image.
        /// </summary>
        /// <returns>The mask.</returns>
        /// <param name="image">The image to use as the mask.</param>
        /// <param name="transparentMeansMasked">If set to <c>true</c> then a transparent pixel means a masked value, otherwise a non transparent pixel means a masked value.</param>
        /// <param name="debugDrawColor">For debugging purposes you can specify a color, which will then add an object to the mask which can drawn on screen (will use the color to draw the mask).</param>
        /// <param name="saveMaskToFile">Whether to save the mask to file (for debugging purposes).</param>
		IMask Load(IBitmap image, bool transparentMeansMasked = false, Color? debugDrawColor = null, 
			string saveMaskToFile = null);

        /// <summary>
        /// Load the mask from the specified 2d array.
        /// </summary>
        /// <returns>The load.</returns>
        /// <param name="mask">Mask.</param>
        /// <param name="id">Identifier.</param>
        /// <param name="inverseMask">If set to <c>true</c> inverse mask.</param>
        /// <param name="debugDrawColor">Debug draw color.</param>
        /// <param name="saveMaskToFile">Save mask to file.</param>
        IMask Load(bool[,] mask, string id, bool inverseMask = false, Color? debugDrawColor = null, string saveMaskToFile = null);
	}
}

