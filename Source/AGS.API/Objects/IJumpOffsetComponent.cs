namespace AGS.API
{
    /// <summary>
    /// Jump offset allows changing the position of the entity without affecting its X,Y and Z co-ordinates.
    /// This means that it won't affect the rendering order, collision tests, etc.
    /// This can be used for having a character jump in the air, for example, without causing it to appear 
    /// behind objects which shouldn't be in front.
    /// This is also used by the engine for scrolling contents inside a scrolling panel.
    /// </summary>
    public interface IJumpOffsetComponent : IComponent
    {
        /// <summary>
        /// Gets or sets the jump offset.
        /// </summary>
        /// <value>The jump offset.</value>
        PointF JumpOffset { get; set; }
    }
}
