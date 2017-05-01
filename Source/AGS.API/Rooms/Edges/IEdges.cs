namespace AGS.API
{
    /// <summary>
    /// Room edges are a convenient way for scripting a room change once a player walks beyond an edge. 
    /// You can set the 'X' for the left and right edge, and the 'Y' for the top and bottom edge and 
    /// subscribe to events when the player crosses the edge to change the room.
    /// </summary>
    public interface IEdges
	{
        /// <summary>
        /// Gets the left edge.
        /// </summary>
        /// <value>The left.</value>
		IEdge Left { get; }

        /// <summary>
        /// Gets the right edge.
        /// </summary>
        /// <value>The right.</value>
		IEdge Right { get; }

        /// <summary>
        /// Gets the top edge.
        /// </summary>
        /// <value>The top.</value>
		IEdge Top { get; }

        /// <summary>
        /// Gets the bottom edge.
        /// </summary>
        /// <value>The bottom.</value>
		IEdge Bottom { get; }
	}
}

