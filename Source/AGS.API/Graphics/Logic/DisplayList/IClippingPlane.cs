using System.Collections.Generic;

namespace AGS.API
{
    /// <summary>
    /// A clipping plane allows clipping objects (i.e hiding objects) if they're on the wrong side of the plane.
    /// A near clipping plane is for clipping objects which are too close to the camera, and a far clipping plane
    /// is for clipping objects which are too far from the camera.
    /// </summary>
    public interface IClippingPlane
    {
        /// <summary>
        /// Gets an object to which the plane will be bound. 
        /// All objects that are to be rendered behind the object will not be shown if the clipping plane in question is a far clipping plane,
        /// and all objects that are to be rendered in front of the object will not be shown if the clipping plane in question is a near clipping plane.
        /// </summary>
        /// <value>The plane object.</value>
        IObject PlaneObject { get; }

		/// <summary>
        /// Gets a value indicating whether the <see cref="PlaneObject"/> (i.e the object representing the plane) 
        /// should be clipped or not.
		/// </summary>
		/// <value><c>true</c> if is plane object clipped; otherwise, <c>false</c>.</value>
		bool IsPlaneObjectClipped { get; }

        /// <summary>
        /// Gets a list of layers on which to check clipping against this plane.
        /// If this is null, all layers will be checked.
        /// </summary>
        /// <value>The layers to clip.</value>
        IReadOnlyList<IRenderLayer> LayersToClip { get; }
    }
}
