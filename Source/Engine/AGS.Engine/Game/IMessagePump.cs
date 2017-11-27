using System.Threading;

namespace AGS.Engine
{
    /// <summary>
    /// Allows to set a synchronization context for specific threads, so we can assign all async task
    /// continuations to continue on the same thread, and invoke actions on a specific thread.
    /// </summary>
	public interface IMessagePump
	{
        /// <summary>
        /// Processes incoming messages (this is called once a tick to ensure all continuations are fired).
        /// </summary>
		void PumpMessages();

        /// <summary>
        /// Sets the synchronization context to the current thread.
        /// </summary>
		void SetSyncContext();

        /// <summary>
        /// Post an action to be performed on the assigned thread (assigned via <see cref="SetSyncContext"/> ).
        /// </summary>
        /// <returns>The post.</returns>
        /// <param name="d">The action to perform.</param>
        /// <param name="state">State.</param>
        void Post(SendOrPostCallback d, object state);
	}

    /// <summary>
    /// The message pump used by the rendering thread.
    /// </summary>
    public interface IRenderMessagePump : IMessagePump {}

    /// <summary>
    /// The message pump used by the update thread.
    /// </summary>
    public interface IUpdateMessagePump : IMessagePump {}
}

