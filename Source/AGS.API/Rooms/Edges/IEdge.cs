namespace AGS.API
{
    /// <summary>
    /// Room edges are a convenient way for scripting a room change once a player walks beyond an edge.
    /// Each room has 4 edges, and this interface represents any of those edges.
    /// </summary>
    public interface IEdge
	{
        /// <summary>
        /// Gets or sets the edge's value 
        /// (if it's left/right edge then it's an X value, if it's top/bottom edge then it's a Y value).
        /// Whether X or Y, the value is used in room coordinates.
        /// </summary>
        /// <value>The value.</value>
		float Value { get; set; }

        /// <summary>
        /// An event which is fired when the player character crosses the edge from inside to outside.
        /// </summary>
        /// <value>The on edge crossed.</value>
		IEvent<object> OnEdgeCrossed { get; }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="T:AGS.API.IEdge"/> is enabled.
        /// If disabled, the <see cref="OnEdgeCrossed"/> event will not be fired on edge crossing.
        /// </summary>
        /// <value><c>true</c> if enabled; otherwise, <c>false</c>.</value>
        bool Enabled { get; set; }
	}
}

