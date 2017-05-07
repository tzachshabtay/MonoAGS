using System.Collections.Generic;

namespace AGS.API
{
    /// <summary>
    /// This implements a path finder, to find a walking path from a position in 2D space to another position,
    /// while a mask implies where the character is allowed to walk on the 2D space.
    /// </summary>
    public interface IPathFinder
	{
        /// <summary>
        /// This gives the path finder the mask which tells where the character is allowed to walk.
        /// </summary>
        /// <param name="mask">Mask.</param>
		void Init(bool[][] mask);

        /// <summary>
        /// Gets the walk points that the character need to travel in straight line to, until eventuall reaching
        /// the destination (an empty list if the travel is not possible).
        /// </summary>
        /// <returns>The walk points.</returns>
        /// <param name="from">From.</param>
        /// <param name="to">To.</param>
		IEnumerable<ILocation> GetWalkPoints(ILocation from, ILocation to);
	}
}

