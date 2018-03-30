using System;
using System.Collections.Generic;

namespace AGS.API
{
    /// <summary>
    /// The render pipeline is responsible for collecting all of the rendering information and passing it on
    /// to the render loop.
    /// It allows entities to subscribe their own renderers and pass on custom drawing instructions.
    /// </summary>
    /// <seealso cref="IRenderer"/>
    /// <seealso cref="IRendererLoop"/>
    /// <seealso cref="IRenderInstruction"/>
    /// <seealso cref="IDisplayList"/>
    public interface IRenderPipeline
    {
        /// <summary>
        /// An event that fires on each tick before processing the display list for rendering.
        /// This allows viewing (and even modifying) the display list prior to rendering it.
        /// </summary>
        /// <returns>The event.</returns>
        IBlockingEvent<DisplayListEventArgs> OnBeforeProcessingDisplayList { get; }

        /// <summary>
        /// Subscribe the specified renderer to the specified entity.
        /// If the entity is part of the display list (i.e if it's in the current room or a GUI), all subscribed
        /// renderers will be called to submit their drawing instructions.
        /// </summary>
        /// <param name="entityID">Entity identifier.</param>
        /// <param name="renderer">Renderer.</param>
        void Subscribe(string entityID, IRenderer renderer);

        /// <summary>
        /// Unsubscribe the specified renderer and entity.
        /// </summary>
        /// <param name="entityID">Entity identifier.</param>
        /// <param name="renderer">Renderer.</param>
        void Unsubscribe(string entityID, IRenderer renderer);
    }
}
