namespace AGS.API
{
    /// <summary>
    /// A 3D location in the world.
    /// </summary>
    public interface ILocation
	{
        /// <summary>
        /// Gets the x coordinate.
        /// </summary>
        /// <value>The x.</value>
		float X { get; }

        /// <summary>
        /// Gets the y coordinate.
        /// </summary>
        /// <value>The y.</value>
		float Y { get; }

        /// <summary>
        /// Gets the z coordinate.
        /// In a 2D world, Z is actually used to decide what gets rendered behind what.
        /// By default, Z will equal Y and will change whenever Y changes, so that characters/objects which are
        /// more to the bottom of the screen will appear in the front, which is the desired behavior in most scenarios.
        /// By setting a location with Z different than Y, this behavior breaks, making Z an independent value (this can
        /// be reverted again by explicitly setting Z to be Y).
        /// </summary>
        /// <value>The z.</value>
	    float Z { get; }

        /// <summary>
        /// Gets the (x,y) as a point.
        /// </summary>
        /// <value>The xy.</value>
		PointF XY { get; }

        void Deconstruct(out float x, out float y, out float z);
	}
}