using System;
using AGS.API;
using System.Threading.Tasks;

namespace AGS.Engine
{
	public class AGSInteractionEvent<TEventArgs> : IEvent<TEventArgs> where TEventArgs : AGSEventArgs
	{
		private readonly IEvent<TEventArgs> _ev;
		private readonly IEvent<TEventArgs> _defaultEvent;

		public AGSInteractionEvent(IEvent<TEventArgs> defaultEvent)
		{
			_ev = new AGSEvent<TEventArgs>();
			_defaultEvent = defaultEvent;
		}

		#region IInteractionEvent implementation

		public int SubscribersCount { get { return _ev.SubscribersCount; } }

		public void Subscribe(Action<object, TEventArgs> callback)
		{
			_ev.Subscribe(callback);
		}

		public void Unsubscribe(Action<object, TEventArgs> callback)
		{
			_ev.Unsubscribe(callback);
		}

		public void WaitUntil(Predicate<TEventArgs> condition)
		{
			if (shouldDefault())
			{
				_defaultEvent.WaitUntil(condition);
			}
			else _ev.WaitUntil(condition);
		}

		public void SubscribeToAsync(Func<object, TEventArgs, Task> callback)
		{
			_ev.SubscribeToAsync(callback);
		}

		public void UnsubscribeToAsync(Func<object, TEventArgs, Task> callback)
		{
			_ev.UnsubscribeToAsync(callback);
		}

		public async Task WaitUntilAsync(Predicate<TEventArgs> condition)
		{
			if (shouldDefault())
			{
				await _defaultEvent.WaitUntilAsync(condition);
			}
			else await _ev.WaitUntilAsync(condition);
		}

		public async Task InvokeAsync(object sender, TEventArgs args)
		{
			if (shouldDefault())
			{
				await _defaultEvent.InvokeAsync(sender, args);
			}
			else await _ev.InvokeAsync(sender, args);
		}

		public void Invoke(object sender, TEventArgs args)
		{
			if (shouldDefault())
			{
				_defaultEvent.Invoke(sender, args);
			}
			else _ev.Invoke(sender, args);
		}

		#endregion

		private bool shouldDefault()
		{
			return _ev.SubscribersCount == 0 && _defaultEvent != null;
		}
	}
}

