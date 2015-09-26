using System;
using System.Threading.Tasks;

namespace AGS.API
{
	public interface IEvent<TEventArgs> where TEventArgs : AGSEventArgs
	{
		int SubscribersCount { get; }

		void Subscribe(Action<object, TEventArgs> callback);
		void Unsubscribe(Action<object, TEventArgs> callback);
		void WaitUntil(Predicate<TEventArgs> condition);

		void SubscribeToAsync(Func<object, TEventArgs, Task> callback);
		void UnsubscribeToAsync(Func<object, TEventArgs, Task> callback);
		Task WaitUntilAsync(Predicate<TEventArgs> condition);

		Task InvokeAsync(object sender, TEventArgs args);
		void Invoke(object sender, TEventArgs args);
	}
}

