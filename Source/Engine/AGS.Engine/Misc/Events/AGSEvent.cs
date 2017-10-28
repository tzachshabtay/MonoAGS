using System;
using AGS.API;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Reflection;

namespace AGS.Engine
{
	public class AGSEvent<TEventArgs> : IEvent<TEventArgs>
	{
		private readonly IConcurrentHashSet<Callback> _invocationList;
		private const int MAX_SUBSCRIPTIONS = 10000;

		public AGSEvent ()
		{
            _invocationList = new AGSConcurrentHashSet<Callback>(fireListChangedEvent: false);
		}

		#region IEvent implementation

		public int SubscribersCount { get { return _invocationList.Count; } }

		public void Subscribe (Action<TEventArgs> callback)
		{
			if (_invocationList.Count > MAX_SUBSCRIPTIONS)
			{
				Debug.WriteLine("Subscribe Overflow!");
				return;
			}
			_invocationList.Add (new Callback (callback));
		}

		public void Unsubscribe (Action<TEventArgs> callback)
		{
			if (!_invocationList.Remove(new Callback (callback)))
			{
			}
		}

		public void SubscribeToAsync (Func<TEventArgs, Task> callback)
		{
			subscribeToAsync(new Callback (callback));
		}

		public void UnsubscribeToAsync (Func<TEventArgs, Task> callback)
		{
			unsubscribeToAsync(new Callback (callback));
		}

		public async Task WaitUntilAsync(Predicate<TEventArgs> condition)
		{
			TaskCompletionSource<object> tcs = new TaskCompletionSource<object> (null);
			var callback = new Callback(condition, tcs);
			subscribeToAsync(callback);
			await tcs.Task;
			unsubscribeToAsync(callback);
		}

		public async Task InvokeAsync (TEventArgs args)
		{
			try
			{
				await Task.Delay(1).ConfigureAwait(false); //Ensuring that the event is invoked on a non-UI thread
				foreach (var target in _invocationList) 
				{
                    if (target.BlockingEvent != null) target.BlockingEvent(args);
					else await target.Event (args);
				}
			}
			catch (Exception e) 
			{
				Debug.WriteLine("Exception when invoking an event asynchronously:");
				Debug.WriteLine (e.ToString ());
				throw;
			}
		}

		public void Invoke (TEventArgs args)
		{
			try
			{
				foreach (var target in _invocationList) 
				{
					if (target.BlockingEvent != null) target.BlockingEvent(args);
					else Task.Run(async () =>  await target.Event(args).ConfigureAwait(true)).Wait();
				}
			}
			catch (Exception e) 
			{
				Debug.WriteLine("Exception when invoking an event:");
				Debug.WriteLine (e.ToString ());
				throw;
			}
		}

		#endregion

		private void subscribeToAsync(Callback callback)
		{
			if (_invocationList.Count > MAX_SUBSCRIPTIONS)
			{
				Debug.WriteLine("Subscribe Overflow!!");
				return;
			}
			_invocationList.Add (callback);
		}

		private void unsubscribeToAsync(Callback callback)
		{
			if (!_invocationList.Remove(callback))
			{
			}
		}

		private class Callback
		{
			private readonly Delegate _origObject;

			public Callback(Func<TEventArgs, Task> callback)
			{
				_origObject = callback;
				Event = callback;
			}

			public Callback(Action<TEventArgs> callback)
			{
				_origObject = callback;
				BlockingEvent = callback;
			}

			public Callback(Predicate<TEventArgs> condition, TaskCompletionSource<object> tcs)
			{
				_origObject = condition;
				Event = convert(condition, tcs);
			}

			public Func<TEventArgs, Task> Event { get; private set; }
			public Action<TEventArgs> BlockingEvent { get; private set; }

			public override bool Equals(object obj)
			{
				Callback other = obj as Callback;
				if (other == null) return false;
				if (other._origObject == _origObject) return true;
				if (_origObject.Target != other._origObject.Target) return false;
				return getMethodName(_origObject) == getMethodName(other._origObject);
			}

			public override int GetHashCode()
			{
				if (_origObject.Target == null) return getMethodName(_origObject).GetHashCode(); //static method subscriptions
				return _origObject.Target.GetHashCode();
			}

			public override string ToString()
			{
				return string.Format("[Event on {0} ({1})]", _origObject.Target.ToString(), getMethodName(_origObject));
			}

			private string getMethodName(Delegate del)
			{
				return RuntimeReflectionExtensions.GetMethodInfo(del).Name;
			}

			private Func<TEventArgs, Task> convert(Predicate<TEventArgs> condition, TaskCompletionSource<object> tcs)
			{
				return e => 
				{
                    if (!condition(e)) return Task.CompletedTask;
					tcs.TrySetResult(null);
                    return Task.CompletedTask;
				};
			}
		}
	}

	public class AGSEvent : IEvent
	{
		private readonly IConcurrentHashSet<Callback> _invocationList;
		private const int MAX_SUBSCRIPTIONS = 10000;

		public AGSEvent()
		{
			_invocationList = new AGSConcurrentHashSet<Callback>(fireListChangedEvent: false);
		}

		#region IEvent implementation

		public int SubscribersCount { get { return _invocationList.Count; } }

		public void Subscribe(Action callback)
		{
			if (_invocationList.Count > MAX_SUBSCRIPTIONS)
			{
				Debug.WriteLine("Subscribe Overflow!");
				return;
			}
			_invocationList.Add(new Callback(callback));
		}

		public void Unsubscribe(Action callback)
		{
			if (!_invocationList.Remove(new Callback(callback)))
			{
			}
		}

		public void SubscribeToAsync(Func<Task> callback)
		{
			subscribeToAsync(new Callback(callback));
		}

		public void UnsubscribeToAsync(Func<Task> callback)
		{
			unsubscribeToAsync(new Callback(callback));
		}

		public async Task WaitUntilAsync(Func<bool> condition)
		{
			TaskCompletionSource<object> tcs = new TaskCompletionSource<object>(null);
			var callback = new Callback(condition, tcs);
			subscribeToAsync(callback);
			await tcs.Task;
			unsubscribeToAsync(callback);
		}

		public async Task InvokeAsync()
		{
			try
			{
				await Task.Delay(1).ConfigureAwait(false); //Ensuring that the event is invoked on a non-UI thread
				foreach (var target in _invocationList)
				{
					await target.Event();
				}
			}
			catch (Exception e)
			{
				Debug.WriteLine("Exception when invoking an event asynchronously:");
				Debug.WriteLine(e.ToString());
				throw;
			}
		}

		public void Invoke()
		{
			try
			{
				foreach (var target in _invocationList)
				{
					if (target.BlockingEvent != null) target.BlockingEvent();
					else Task.Run(async () => await target.Event().ConfigureAwait(true)).Wait();
				}
			}
			catch (Exception e)
			{
				Debug.WriteLine("Exception when invoking an event:");
				Debug.WriteLine(e.ToString());
				throw;
			}
		}

		#endregion

		private void subscribeToAsync(Callback callback)
		{
			if (_invocationList.Count > MAX_SUBSCRIPTIONS)
			{
				Debug.WriteLine("Subscribe Overflow!!");
				return;
			}
			_invocationList.Add(callback);
		}

		private void unsubscribeToAsync(Callback callback)
		{
			if (!_invocationList.Remove(callback))
			{
			}
		}

		private class Callback
		{
			private readonly Delegate _origObject;

			public Callback(Func<Task> callback)
			{
				_origObject = callback;
				Event = callback;
			}

			public Callback(Action callback)
			{
				_origObject = callback;
				Event = convert(callback);
				BlockingEvent = callback;
			}

			public Callback(Func<bool> condition, TaskCompletionSource<object> tcs)
			{
				_origObject = condition;
				Event = convert(condition, tcs);
			}

			public Func<Task> Event { get; private set; }
			public Action BlockingEvent { get; private set; }

			public override bool Equals(object obj)
			{
				Callback other = obj as Callback;
				if (other == null) return false;
				if (other._origObject == _origObject) return true;
				if (_origObject.Target != other._origObject.Target) return false;
				return getMethodName(_origObject) == getMethodName(other._origObject);
			}

			public override int GetHashCode()
			{
				if (_origObject.Target == null) return getMethodName(_origObject).GetHashCode(); //static method subscriptions
				return _origObject.Target.GetHashCode();
			}

			public override string ToString()
			{
				return string.Format("[Event on {0} ({1})]", _origObject.Target.ToString(), getMethodName(_origObject));
			}

			private string getMethodName(Delegate del)
			{
				return RuntimeReflectionExtensions.GetMethodInfo(del).Name;
			}

			private Func<Task> convert(Action callback)
			{
				return () =>
				{
					callback();
					return Task.CompletedTask;
				};
			}

			private Func<Task> convert(Func<bool> condition, TaskCompletionSource<object> tcs)
			{
				return () =>
				{
					if (!condition()) return Task.CompletedTask;
					tcs.TrySetResult(null);
					return Task.CompletedTask;
				};
			}
		}
	}
}

