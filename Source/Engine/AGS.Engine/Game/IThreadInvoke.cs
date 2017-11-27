using System;

namespace AGS.Engine
{
    /// <summary>
    /// Allows to invoke actions on a specific thread.
    /// </summary>
    public interface IThreadInvoke
    {
        void RunBlocking(Action action);
    }

    /// <summary>
    /// Allows to invoke actions on the render thread.
    /// </summary>
    public interface IRenderThread : IThreadInvoke {}

    /// <summary>
    /// Allows to invoke actions on the update thread.
    /// </summary>
    public interface IUpdateThread : IThreadInvoke {}
}
