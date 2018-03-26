using System;
namespace AGS.API
{
    /// <summary>
    /// Allows getting the world position for entities.
    /// </summary>
    public interface IWorldPositionComponent : IComponent
    {
        /// <summary>
        /// Get the X of an entity in world coordinates.
        /// In case the entity has no parents, the value will be the same as <see cref="ITranslate.X"/>,
        /// but if the entity has a parent, then WorldX will give you the absolute world value (as opposed to relative to the parent).
        /// </summary>
        /// <value>The world x.</value>
        float WorldX { get; }

        /// <summary>
        /// Get the Y of an entity in world coordinates.
        /// In case the entity has no parents, the value will be the same as <see cref="ITranslate.Y"/>,
        /// but if the entity has a parent, then WorldY will give you the absolute world value (as opposed to relative to the parent).
        /// </summary>
        /// <value>The world y.</value>
        float WorldY { get; }

        /// <summary>
        /// Get the (X,Y) of an entity in world coordinates.
        /// In case the entity has no parents, the value will be the same as <see cref="ITranslate.Location"/>,
        /// but if the entity has a parent, then WorldXY will give you the absolute world value (as opposed to relative to the parent).
        /// </summary>
        /// <value>The world y.</value>
        PointF WorldXY { get; }
    }
}