namespace AGS.API
{
    /// <summary>
    /// Areas are specific regions in a room that provide additional behaviors that apply only within that region.
    /// </summary>
    /// <seealso cref="IWalkableArea"/>
    /// <seealso cref="IWalkBehindArea"/>
    /// <seealso cref="IScalingArea"/>
    /// <seealso cref="IZoomArea"/>
    /// <seealso cref="IAreaRestriction"/>
    [RequiredComponent(typeof(ITranslateComponent), false)]
    [RequiredComponent(typeof(IRotateComponent), false)]
    public interface IAreaComponent : IComponent
	{
        /// <summary>
        /// Gets or sets the mask which defines the area (each cell with 1 in the mask is part of the area).
        /// </summary>
        /// <value>The mask.</value>
		IMask Mask { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="T:AGS.API.IAreaComponent"/> is enabled.
        /// This would affect all the components attached to this area (so if an area is both a walkable area and a scaling area, 
        /// having it disabled will disable both). 
        /// </summary>
        /// <value><c>true</c> if enabled; otherwise, <c>false</c>.</value>
		bool Enabled { get; set; }

        /// <summary>
        /// Is the specified point inside the area?
        /// </summary>
        /// <returns><c>true</c>, if is inside the area, <c>false</c> otherwise.</returns>
        /// <param name="point">Point.</param>
		bool IsInArea(PointF point);

        /// <summary>
        /// Is the specified point inside the area after being projected and scaled?
        /// </summary>
        /// <returns><c>true</c>, if point is inside area, <c>false</c> otherwise.</returns>
        /// <param name="point">Point.</param>
        /// <param name="projectionBox">Projection box.</param>
        /// <param name="scaleX">Scale x.</param>
        /// <param name="scaleY">Scale y.</param>
        bool IsInArea(PointF point, AGSBoundingBox projectionBox, float scaleX, float scaleY);

        /// <summary>
        /// Finds the closest point in the area to the specified point, and also
        /// returns the distance between the points.
        /// </summary>
        /// <returns>The closest point.</returns>
        /// <param name="point">Point.</param>
        /// <param name="distance">Distance.</param>
		PointF? FindClosestPoint(PointF point, out float distance);
	}

    /// <summary>
    /// Areas are specific regions in a room that provide additional behaviors that apply only within that region.
    /// </summary>
    /// <seealso cref="IWalkableArea"/>
    /// <seealso cref="IWalkBehindArea"/>
    /// <seealso cref="IScalingArea"/>
    /// <seealso cref="IZoomArea"/>
    /// <seealso cref="IAreaRestriction"/>
    public interface IArea : IEntity, IAreaComponent { }
}

