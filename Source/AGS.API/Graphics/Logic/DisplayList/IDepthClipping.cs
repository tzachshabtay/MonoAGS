namespace AGS.API
{
	/// <summary>
	/// Depth clipping allows clipping objects (i.e hiding objects) if they're either too close or too far from the camera.
	/// A near clipping plane is for clipping objects which are too close to the camera, and a far clipping plane
	/// is for clipping objects which are too far from the camera.
	/// </summary>
	public interface IDepthClipping
    {
        /// <summary>
        /// Gets or sets the near clipping plane (objects in front of the plane will be hidden from the camera).
        /// The default is null, meaning no near clipping is taking place.
        /// </summary>
        /// <value>The near clipping plane.</value>
        IClippingPlane NearClippingPlane { get; set; }

		/// <summary>
		/// Gets or sets the far clipping plane (objects behind of the plane will be hidden from the camera).
		/// The default is null, meaning no far clipping is taking place.
		/// </summary>
		/// <value>The near clipping plane.</value>
		IClippingPlane FarClippingPlane { get; set; }

        /// <summary>
        /// Checkes whether an object is clipped (i.e should be hidden) by one of the clipping planes.
        /// </summary>
        /// <returns><c>true</c>, if object should be clipped, <c>false</c> otherwise.</returns>
        /// <param name="obj">Object.</param>
        bool IsObjectClipped(IObject obj);
    }
}
