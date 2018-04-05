using System;
namespace AGS.API
{
    /// <summary>
    /// Allows drawing a border around the entity.
    /// </summary>
    public interface IBorderComponent : IComponent
    {
        /// <summary>
        /// Gets or sets a border that will (optionally) surround the entity.
        /// </summary>
        /// <value>The border.</value>
        IBorderStyle Border { get; set; }
    }
}