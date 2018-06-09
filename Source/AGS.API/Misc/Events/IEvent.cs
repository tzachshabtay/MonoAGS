using System;
using System.Threading.Tasks;

namespace AGS.API
{
    /// <summary>
    /// Represents an event which can be subscribed both synchronously or asynchorously, and is invoked asynchronously.
    /// An event is a notification for something that has happened.
    /// Interested parties can subscribe to the event and be notified when it triggers (https://en.wikipedia.org/wiki/Event-driven_programming).
    /// </summary>
	public interface IEvent<TEventArgs> : IDisposable
	{
        /// <summary>
        /// Gets the number of subscribers to the event.
        /// </summary>
        /// <value>The subscribers count.</value>
        int SubscribersCount { get; }

        /// <summary>
        /// Subscribe the specified callback to the event.
        /// Once subscribed, whenever the event happens this callback will be called.
        /// </summary>
        /// <param name="callback">Callback.</param>
        /// <param name="priority">The callback priority (determines the order in which the subscribers get the events).</param>
        void Subscribe(Action<TEventArgs> callback, CallbackPriority priority = CallbackPriority.Normal);

        /// <summary>
        /// Unsubscribe the specified callback from the event.
        /// This will stops notifications to call this callback.
        /// </summary>
        /// <param name="callback">Callback.</param>
        /// <param name="priority">The callback priority (determines the order in which the subscribers get the events).</param>
        void Unsubscribe(Action<TEventArgs> callback, CallbackPriority priority = CallbackPriority.Normal);

        /// <summary>
        /// Subscribe the specified asynchronous callback to the event.
        /// Once subscribed, whenever the event happens this callback will be called.
        /// </summary>
        /// <param name="callback">Callback.</param>
        /// <param name="priority">The callback priority (determines the order in which the subscribers get the events).</param>
        void SubscribeToAsync(Func<TEventArgs, Task> callback, CallbackPriority priority = CallbackPriority.Normal);

        /// <summary>
        /// Unsubscribe the specified asynchronous callback from the event.
        /// This will stops notifications to call this callback.
        /// </summary>
        /// <param name="callback">Callback.</param>
        /// <param name="priority">The callback priority (determines the order in which the subscribers get the events).</param>
        void UnsubscribeToAsync(Func<TEventArgs, Task> callback, CallbackPriority priority = CallbackPriority.Normal);

        /// <summary>
        /// Subscribe the specified callback to the event.
        /// Once subscribed, whenever the event happens this callback will be called.
        /// This version of Subscribe ignores incoming arguments from the event (if you need the arguments, use the other overload which gets an action of TEventArgs).
        /// </summary>
        /// <param name="callback">Callback.</param>
        /// <param name="priority">The callback priority (determines the order in which the subscribers get the events).</param>
        void Subscribe(Action callback, CallbackPriority priority = CallbackPriority.Normal);

        /// <summary>
        /// Unsubscribe the specified callback from the event.
        /// This will stops notifications to call this callback.
        /// </summary>
        /// <param name="callback">Callback.</param>
        /// <param name="priority">The callback priority (determines the order in which the subscribers get the events).</param>
        void Unsubscribe(Action callback, CallbackPriority priority = CallbackPriority.Normal);

        /// <summary>
        /// Subscribe the specified asynchronous callback to the event.
        /// Once subscribed, whenever the event happens this callback will be called.
        /// This version of Subscribe ignores incoming arguments from the event (if you need the arguments, use the other overload which gets a func of TEventArgs -> Task).
        /// </summary>
        /// <param name="callback">Callback.</param>
        /// <param name="priority">The callback priority (determines the order in which the subscribers get the events).</param>
        void SubscribeToAsync(Func<Task> callback, CallbackPriority priority = CallbackPriority.Normal);

        /// <summary>
        /// Unsubscribe the specified asynchronous callback from the event.
        /// This will stops notifications to call this callback.
        /// </summary>
        /// <param name="callback">Callback.</param>
        /// <param name="priority">The callback priority (determines the order in which the subscribers get the events).</param>
        void UnsubscribeToAsync(Func<Task> callback, CallbackPriority priority = CallbackPriority.Normal);

        /// <summary>
        /// Asynchronously wait until the event fires and the specific condition applies.
        /// </summary>
        /// <returns>The task to be awaited.</returns>
        /// <param name="condition">The condition we are waiting to apply before moving on.</param>
        /// <param name="priority">The callback priority (determines the order in which the subscribers get the events).</param>
        Task WaitUntilAsync(Predicate<TEventArgs> condition, CallbackPriority priority = CallbackPriority.Normal);

        /// <summary>
        /// Invoke the event asynchronously.
        /// </summary>
        /// <param name="args">Event arguments which can be used to provide additional data.</param>
		Task InvokeAsync(TEventArgs args);
	}

    /// <summary>
    /// Represents an event which can be subscribed both synchronously or asynchorously, and is invoked asynchronously.
    /// An event is a notification for something that has happened.
    /// Interested parties can subscribe to the event and be notified when it triggers (https://en.wikipedia.org/wiki/Event-driven_programming).
    /// </summary>
    public interface IEvent : IDisposable
    {
        /// <summary>
        /// Gets the number of subscribers to the event.
        /// </summary>
        /// <value>The subscribers count.</value>
        int SubscribersCount { get; }

        /// <summary>
        /// Subscribe the specified callback to the event.
        /// Once subscribed, whenever the event happens this callback will be called.
        /// </summary>
        /// <param name="callback">Callback.</param>
        /// <param name="priority">The callback priority (determines the order in which the subscribers get the events).</param>
        void Subscribe(Action callback, CallbackPriority priority = CallbackPriority.Normal);

        /// <summary>
        /// Unsubscribe the specified callback from the event.
        /// This will stops notifications to call this callback.
        /// </summary>
        /// <param name="callback">Callback.</param>
        /// <param name="priority">The callback priority (determines the order in which the subscribers get the events).</param>
        void Unsubscribe(Action callback, CallbackPriority priority = CallbackPriority.Normal);

        /// <summary>
        /// Subscribe the specified asynchronous callback to the event.
        /// Once subscribed, whenever the event happens this callback will be called.
        /// </summary>
        /// <param name="callback">Callback.</param>
        /// <param name="priority">The callback priority (determines the order in which the subscribers get the events).</param>
        void SubscribeToAsync(Func<Task> callback, CallbackPriority priority = CallbackPriority.Normal);

        /// <summary>
        /// Unsubscribe the specified asynchronous callback from the event.
        /// This will stops notifications to call this callback.
        /// </summary>
        /// <param name="callback">Callback.</param>
        /// <param name="priority">The callback priority (determines the order in which the subscribers get the events).</param>
        void UnsubscribeToAsync(Func<Task> callback, CallbackPriority priority = CallbackPriority.Normal);

        /// <summary>
        /// Asynchronously wait until the event fires and the specific condition applies.
        /// </summary>
        /// <returns>The task to be awaited.</returns>
        /// <param name="condition">The condition we are waiting to apply before moving on.</param>
        /// <param name="priority">The callback priority (determines the order in which the subscribers get the events).</param>
        Task WaitUntilAsync(Func<bool> condition, CallbackPriority priority = CallbackPriority.Normal);

        /// <summary>
        /// Invoke the event asynchronously.
        /// </summary>
        Task InvokeAsync();
    }
}

