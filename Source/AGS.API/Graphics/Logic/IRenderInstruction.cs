namespace AGS.API
{
    /// <summary>
    /// A render instruction represents an instruction to draw something.
    /// It is created on demand by renderers, and used by the rendering loop to draw on the screen on each game tick.
    /// </summary>
    /// <seealso cref="IRenderPipeline"/>
    /// <seealso cref="IRenderer"/>
    /// <seealso cref="IRendererLoop"/>
    public interface IRenderInstruction
    {
        /// <summary>
        /// Draw something.
        /// </summary>
        void Render();

        /// <summary>
        /// After the rendering instruction has completed, the rendering loop tells the render instruction that it's
        /// not needed anymore by calling the "Release" function. 
        /// The instruction can then recycle itself (if object pooling is used) to reduce memory allocations.
        /// </summary>
        void Release();
    }
}
