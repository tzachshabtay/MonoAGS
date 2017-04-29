using System.Collections.Generic;

namespace AGS.API
{
    /// <summary>
    /// Allows grouping animations for different directions together (for example, walking left and right).
    /// This is used by the character when choosing a directional animation: if you don't assign all of the
    /// directions, the engine will attempt to choose the best direction based on what you have assigned.
    /// So, for example, if you only assign Left and Right animations for your walk animation, and the player
    /// is attempting to walk down-right, the engine will use the Right animation.
    /// </summary>
    public interface IDirectionalAnimation
	{
        /// <summary>
        /// Gets or sets the left direction animation.
        /// </summary>
        /// <value>The left direction animation.</value>
		IAnimation Left { get; set; }

        /// <summary>
        /// Gets or sets the right direction animation.
        /// </summary>
        /// <value>The right direction animation.</value>
		IAnimation Right { get; set; }

        /// <summary>
        /// Gets or sets the up direction animation.
        /// </summary>
        /// <value>The up direction animation.</value>
		IAnimation Up { get; set; }

        /// <summary>
        /// Gets or sets the down direction animation.
        /// </summary>
        /// <value>The down direction animation.</value>
		IAnimation Down { get; set; }

        /// <summary>
        /// Gets or sets the up and left direction animation.
        /// </summary>
        /// <value>The up and left direction animation.</value>
		IAnimation UpLeft { get; set; }

        /// <summary>
        /// Gets or sets the up and right direction animation.
        /// </summary>
        /// <value>The up and right direction animation.</value>
		IAnimation UpRight { get; set; }

        /// <summary>
        /// Gets or sets the down and left direction animation.
        /// </summary>
        /// <value>The down and left direction animation.</value>
		IAnimation DownLeft { get; set; }

        /// <summary>
        /// Gets or sets the down and right direction animation.
        /// </summary>
        /// <value>The down and right direction animation.</value>
		IAnimation DownRight { get; set; }

        /// <summary>
        /// Allows to iterate on all assigned directions.
        /// </summary>
        /// <returns>All assigned directions.</returns>
        IEnumerable<IAnimation> GetAllDirections();
	}
}

