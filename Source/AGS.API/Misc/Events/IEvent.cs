using System;
using System.Threading.Tasks;

namespace AGS.API
{
    /// <summary>
    /// Represents an event which can be subscribed and invoked, both synchronously and asynchronously.
    /// An event is a notification for something that has happened.
    /// Interested parties can subscribe to the event and be notified when it triggers (https://en.wikipedia.org/wiki/Event-driven_programming).
    /// </summary>
	public interface IEvent<TEventArgs> : IBlockingEvent<TEventArgs>
	{
        /// <summary>
        /// Subscribe the specified asynchronous callback to the event.
        /// Once subscribed, whenever the event happens this callback will be called.
        /// </summary>
        /// <param name="callback">Callback.</param>
		void SubscribeToAsync(Func<TEventArgs, Task> callback);

        /// <summary>
        /// Unsubscribe the specified asynchronous callback from the event.
        /// This will stops notifications to call this callback.
        /// </summary>
        /// <param name="callback">Callback.</param>
		void UnsubscribeToAsync(Func<TEventArgs, Task> callback);

        /// <summary>
        /// Asynchronously wait until the event fires and the specific condition applies.
        /// </summary>
        /// <returns>The task to be awaited.</returns>
        /// <param name="condition">The condition we are waiting to apply before moving on.</param>
		Task WaitUntilAsync(Predicate<TEventArgs> condition);

        /// <summary>
        /// Invoke the event asynchronously.
        /// </summary>
        /// <param name="args">Event arguments which can be used to provide additional data.</param>
		Task InvokeAsync(TEventArgs args);
	}
}

