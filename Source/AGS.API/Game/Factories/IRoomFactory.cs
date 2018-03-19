using System.Threading.Tasks;

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

        /// <summary>
        /// Creates an area, which can be made walkable and/or walk-behind (or none of the above).
        /// A walkable area is an area in which characters can walk.
        /// A walk-behind area is an area which puts the characters behind the room's background, if their Y property
        /// is under the walk-behind area's baseline (usually the bottom of the mask). This is useful if part of your
        /// background image contains spots which should actually be in the front.
        /// </summary>
        /// <returns>The area.</returns>
        /// <param name="maskPath">The resource path which contains the bitmap mask for the area.</param>
        /// <param name="room">The room to place the area in.</param>
        /// <param name="isWalkable">If set to <c>true</c> the area should be made walkable.</param>
        /// <param name="isWalkBehind">If set to <c>true</c> the should be made a walk-behind area.</param>
        IArea GetArea(string maskPath, IRoom room = null, bool isWalkable = false, bool isWalkBehind = false);

		/// <summary>
		/// Creates an area from a mask, which can be made walkable and/or walk-behind (or none of the above).
		/// A walkable area is an area in which characters can walk.
		/// A walk-behind area is an area which puts the characters behind the room's background, if their Y property
		/// is under the walk-behind area's baseline (usually the bottom of the mask). This is useful if part of your
		/// background image contains spots which should actually be in the front.
		/// </summary>
		/// <returns>The area.</returns>
		/// <param name="id">Unique identifier for the area.</param>
		/// <param name="mask">Mask.</param>
        /// <param name="room">The room to place the area in.</param>
		/// <param name="isWalkable">If set to <c>true</c> is walkable.</param>
		/// <param name="isWalkBehind">If set to <c>true</c> is walk behind.</param>
		IArea GetArea(string id, IMask mask, IRoom room = null, bool isWalkable = false, bool isWalkBehind = false);

        /// <summary>
        /// Creates an area asynchronously, which can be made walkable and/or walk-behind (or none of the above).
        /// A walkable area is an area in which characters can walk.
        /// A walk-behind area is an area which puts the characters behind the room's background, if their Y property
        /// is under the walk-behind area's baseline (usually the bottom of the mask). This is useful if part of your
        /// background image contains spots which should actually be in the front.
        /// </summary>
        /// <returns>The area.</returns>
        /// <param name="maskPath">The resource path which contains the bitmap mask for the area.</param>
        /// <param name="room">The room to place the area in.</param>
        /// <param name="isWalkable">If set to <c>true</c> the area should be made walkable.</param>
        /// <param name="isWalkBehind">If set to <c>true</c> the should be made a walk-behind area.</param>
        Task<IArea> GetAreaAsync(string maskPath, IRoom room = null, bool isWalkable = false, bool isWalkBehind = false);

        /// <summary>
        /// Creates a scaling area(<see cref="IScalingArea"/>). 
        /// </summary>
        /// <param name="area">The area that will be "transormed" into a scaling area.</param>
        /// <param name="minScaling">Minimum scaling (will be applied at the top of the area).</param>
        /// <param name="maxScaling">Maximum scaling (will be applied at the bottom of the area).</param>
        /// <param name="scaleObjectsX">If set to <c>true</c> scale objects on the horizontal axis while in this area.</param>
        /// <param name="scaleObjectsY">If set to <c>true</c> scale objects on the vertical axis while in this area.</param>
        /// <param name="scaleVolume">If set to <c>true</c> scale volume emitted from objects when in this area.</param>
        void CreateScaleArea(IArea area, float minScaling, float maxScaling, bool scaleObjectsX = true, bool scaleObjectsY = true, bool scaleVolume = true);

        /// <summary>
        /// Creates a camera zoom area(<see cref="IZoomArea"/>).
        /// </summary>
        /// <param name="area">The area that will be "transformed" into a zoom area.</param>
        /// <param name="minZoom">Minimum zoom factor (will be applied at the bottom of the area).</param>
        /// <param name="maxZoom">Maximum zoom factor (will be applied at the top of the area).</param>
        void CreateZoomArea(IArea area, float minZoom, float maxZoom);
	}
}