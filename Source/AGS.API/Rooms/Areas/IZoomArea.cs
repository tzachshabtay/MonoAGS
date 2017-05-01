namespace AGS.API
{
    /// <summary>
    /// These are areas that indicate the camera should automatically zoom when the player (or whatever other target 
    /// was chosen for the camera) is in that area. 
    /// This works along nicely with a vertical scaling area, to zoom the camera as the player moves farther 
    /// away from the camera.
    /// </summary>
    [RequiredComponent(typeof(IArea))]
    public interface IZoomArea : IComponent
    {
        /// <summary>
        /// The lowest point in the area, which usually corresponds to the closest distance to the camera 
        /// (if your background has perspective) will have the camera at its "minimal zoom" which is what configured
        /// here. This will be interpolated as the character moves up the area until it reaches the top-most area (the "maximal zoom").
        /// </summary>
        /// <value>The minimum zoom.</value>
        float MinZoom { get; set; }

        /// <summary>
        /// The highest point in the area, which usually corresponds to the farthest distance to the camera 
        /// (if your background has perspective) will have the camera at its "maximal zoom" which is what configured
        /// here. This will be interpolated as the character moves down the area until it reaches the most bottom area (the "minimal zoom").
        /// </summary>
        /// <value>The minimum zoom.</value>
        float MaxZoom { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to zoom the camera on this area or not.
        /// </summary>
        /// <value><c>true</c> if zoom camera; otherwise, <c>false</c>.</value>
        bool ZoomCamera { get; set; }

        /// <summary>
        /// Gets the zoom factor for the given Y value (in room coordinates).
        /// </summary>
        /// <returns>The zoom.</returns>
        /// <param name="value">Value.</param>
        float GetZoom(float value);
    }
}
