namespace AGS.API
{
    /// <summary>
    /// Factory for creating rooms and room specifics.
    /// </summary>
    public interface IRoomFactory
	{
        /// <summary>
        /// Creates a new room edge.
        /// </summary>
        /// <returns>The edge.</returns>
        /// <param name="value">The value for the edge (if this is a left edge, the the left value, for a top edge, the top value, etc).</param>
		IEdge GetEdge(float value = 0f);

        /// <summary>
        /// Creates a new room.
        /// </summary>
        /// <returns>The room.</returns>
        /// <param name="id">A unique identifier for the room.</param>
        /// <param name="leftEdge">Left edge.</param>
        /// <param name="rightEdge">Right edge.</param>
        /// <param name="bottomEdge">Bottom edge.</param>
        /// <param name="topEdge">Top edge.</param>
		IRoom GetRoom(string id, float leftEdge = 0f, float rightEdge = 0f, float bottomEdge = 0f, float topEdge = 0f);
	}
}

