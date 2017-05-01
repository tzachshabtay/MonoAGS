using System.Threading.Tasks;

namespace AGS.API
{
    /// <summary>
    /// Allows saving/loading a game.
    /// </summary>
    public interface ISaveLoad
	{
        /// <summary>
        /// Save the game with the specified saveName.
        /// </summary>
        /// <param name="saveName">Save name.</param>
		void Save(string saveName);

        /// <summary>
        /// Save the game asynchronously with the specified saveName.
        /// </summary>
        /// <param name="saveName">Save name.</param>
		Task SaveAsync(string saveName);

        /// <summary>
        /// Load the game with the specified saveName.
        /// </summary>
        /// <returns>The load.</returns>
        /// <param name="saveName">Save name.</param>
		void Load(string saveName);

        /// <summary>
        /// Load the game asynchronously with the specified saveName.
        /// </summary>
        /// <returns>The load.</returns>
        /// <param name="saveName">Save name.</param>
		Task LoadAsync(string saveName);

        /// <summary>
        /// Sets the restart point. This will save the game and will be loaded when restarting the game.
        /// </summary>
        /// <seealso cref="Restart"/>
		void SetRestartPoint();

        /// <summary>
        /// Restarts the game by loading the last set restart point.
        /// </summary>
        /// <seealso cref="SetRestartPoint"/>
		void Restart();
	}
}

