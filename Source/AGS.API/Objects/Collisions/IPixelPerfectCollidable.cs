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

        /// <summary>
        /// Enables/disables pixel perfect collision checks (disabling will resort to a bounding box collision check).
        /// </summary>
        /// <param name="pixelPerfect">If set to <c>true</c> pixel perfect.</param>
        void PixelPerfect(bool pixelPerfect);
    }

    /// <summary>
    /// Adds the ability for an entity to have pixel perfect collision checks (as opposed to bounding box collision checks).
    /// See: https://wiki.allegro.cc/index.php?title=Pixel_Perfect_Collision
    /// </summary>
    [RequiredComponent(typeof(IAnimationComponent))]
    public interface IPixelPerfectComponent : IPixelPerfectCollidable, IComponent
    { }
}
