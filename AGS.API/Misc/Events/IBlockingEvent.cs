using System;

namespace AGS.API
{
	public interface IBlockingEvent<TEventArgs> where TEventArgs : AGSEventArgs
	{
		int SubscribersCount { get; }

		void Subscribe(Action<object, TEventArgs> callback);
		void Unsubscribe(Action<object, TEventArgs> callback);

		void Invoke(object sender, TEventArgs args);
	}
}

