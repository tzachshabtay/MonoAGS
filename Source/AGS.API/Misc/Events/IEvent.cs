using System;
using System.Threading.Tasks;

namespace AGS.API
{
	public interface IEvent<TEventArgs> : IBlockingEvent<TEventArgs> where TEventArgs : AGSEventArgs
	{
		void WaitUntil(Predicate<TEventArgs> condition);

		void SubscribeToAsync(Func<object, TEventArgs, Task> callback);
		void UnsubscribeToAsync(Func<object, TEventArgs, Task> callback);
		Task WaitUntilAsync(Predicate<TEventArgs> condition);

		Task InvokeAsync(object sender, TEventArgs args);
	}
}

