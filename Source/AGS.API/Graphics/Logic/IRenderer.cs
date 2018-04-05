using System;
namespace AGS.API
{
    /// <summary>
    /// A renderer is an object which passes on drawing instructions on demand.
    /// All renderers which are subcribed to the rendering pipeline (for entities which are to be displayed),
    /// are requested to pass on the drawing instructions on each game tick.
    /// The rendering loop then uses those instructions to draw on the screen.
    /// </summary>
    /// <seealso cref="IRenderPipeline"/>
    /// <seealso cref="IRenderInstruction"/>
    /// <seealso cref="IRendererLoop"/>
    public interface IRenderer
    {
        /// <summary>
        /// Gets the next drawing instruction for the specified viewport
        /// </summary>
        /// <returns>The next instruction.</returns>
        /// <param name="viewport">Viewport.</param>
        IRenderInstruction GetNextInstruction(IViewport viewport);
    }
}
