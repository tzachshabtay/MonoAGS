namespace AGS.API
{
    /// <summary>
    /// This component allows cropping all children to the parent's size.
    /// For example, you can have an object to represent a TV, then crop all of its children entities,
    /// so the entities will not get out of the boundaries of the TV (giving the appearance that it's really a TV).
    /// </summary>
    [RequiredComponent(typeof(IInObjectTree))]
    [RequiredComponent(typeof(IScaleComponent))]
    [RequiredComponent(typeof(ICollider))]
    public interface ICropChildrenComponent : IComponent
    {
        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="T:AGS.API.ICropChildrenComponent"/> crop children is enabled.
        /// </summary>
        /// <value><c>true</c> if crop children enabled; otherwise, <c>false</c>.</value>
        bool CropChildrenEnabled { get; set; }

        /// <summary>
        /// Gets or sets the start point for the crop area, where (0,0) is the top-left corner.
        /// </summary>
        /// <value>The start point.</value>
        PointF StartPoint { get; set; }
    }
}
