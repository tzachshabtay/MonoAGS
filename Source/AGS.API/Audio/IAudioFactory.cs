using System.Threading.Tasks;

namespace AGS.API
{
    /// <summary>
    /// A factory to load audio clips from files.
    /// </summary>
    public interface IAudioFactory
	{
        /// <summary>
        /// Loads the audio clip from the file/resource path.
        /// </summary>
        /// <returns>The audio clip.</returns>
        /// <param name="filePath">File path.</param>
        /// <param name="id">A unique identifier for the clip, if null, the file/resource path will be used as id.</param>
		IAudioClip LoadAudioClip(string filePath, string id = null);

        /// <summary>
        /// Loads the audio clip from the file/resource path asynchronously.
        /// </summary>
        /// <returns>The audio clip.</returns>
        /// <param name="filePath">File path.</param>
        /// <param name="id">A unique identifier for the clip, if null, the file/resource path will be used as id.</param>
		Task<IAudioClip> LoadAudioClipAsync(string filePath, string id = null);
	}
}

