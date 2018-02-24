using System;

namespace AGS.API
{
    /// <summary>
    /// Adds the ability for pixel perfect collision checks (https://wiki.allegro.cc/index.php?title=Pixel_Perfect_Collision)
    /// </summary>
    public interface IPixelPerfectCollidable : IDisposable
    {
        /// <summary>
        /// Gets the pixel perfect hit test area.
        /// This is the area not projected on the screen(i.e not rotated and will start from 0,0)
        /// </summary>
        /// <value>The pixel perfect hit test area.</value>
        IArea PixelPerfectHitTestArea { get; }
    }

    /// <summary>
    /// Adds the ability for an entity to have pixel perfect collision checks (as opposed to bounding box collision checks).
    /// See: https://wiki.allegro.cc/index.php?title=Pixel_Perfect_Collision
    /// </summary>
    [RequiredComponent(typeof(IAnimationComponent))]
    public interface IPixelPerfectComponent : IPixelPerfectCollidable, IComponent
    {
        bool IsPixelPerfect { get; set; }
    }
}
