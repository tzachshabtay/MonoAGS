using System.Threading.Tasks;

namespace AGS.API
{
    /// <summary>
    /// The speech cache is responsible for providing the correct audio clips to play for a specific text 
    /// that's being said.
    /// </summary>
    public interface ISpeechCache
    {
        /// <summary>
        /// Gets the audio clip to play and the text to write on screen for the specific said text.
        /// </summary>
        /// <returns>The audio.</returns>
        /// <param name="characterName">The name of the character that speaks the text</param>
        /// <param name="text">The said text.</param>
        Task<ISpeechLine> GetSpeechLineAsync(string characterName, string text);
    }
}

