using System.Threading.Tasks;

namespace AGS.API
{
    /// <summary>
    /// The game loop, which updates the game state on every tick.
    /// </summary>
    public interface IGameLoop
	{
        /// <summary>
        /// Updates the state (this trigger animation moves, walking, etc). 
        /// This is called repeatedly by the engine, and usually should not be used by the user.
        /// </summary>
        /// <returns>The update task.</returns>
		void Update();
	}
}

