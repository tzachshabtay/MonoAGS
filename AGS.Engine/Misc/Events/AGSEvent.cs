using System;
using AGS.API;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;

namespace AGS.Engine
{
	public class AGSEvent<TEventArgs> : IEvent<TEventArgs> where TEventArgs : AGSEventArgs
	{
		private readonly Guid _id;
		private readonly IConcurrentHashSet<Callback> _invocationList;
		private const int MAX_SUBSCRIPTIONS = 10000;

		public AGSEvent ()
		{
			_id = Guid.NewGuid();
			_invocationList = new AGSConcurrentHashSet<Callback> ();
		}

		#region IEvent implementation

		public int SubscribersCount { get { return _invocationList.Count; } }

		public void Subscribe (Action<object, TEventArgs> callback)
		{
			if (_invocationList.Count > MAX_SUBSCRIPTIONS)
			{
				Debug.WriteLine("Subscribe Overflow!");
				return;
			}
			_invocationList.Add (new Callback (callback));
		}

		public void Unsubscribe (Action<object, TEventArgs> callback)
		{
			if (!_invocationList.Remove(new Callback (callback)))
			{
			}
		}

		public void WaitUntil(Predicate<TEventArgs> condition)
		{
			Task.Run(async () => await WaitUntilAsync(condition)).Wait();
		}

		public void SubscribeToAsync (Func<object, TEventArgs, Task> callback)
		{
			subscribeToAsync(new Callback (callback));
		}

		public void UnsubscribeToAsync (Func<object, TEventArgs, Task> callback)
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

		public async Task InvokeAsync (object sender, TEventArgs args)
		{
			try
			{
				if (args != null)
					args.TimesInvoked = Repeat.Do(_id.ToString());
				foreach (var target in _invocationList) 
				{
					await target.Event (sender, args).ConfigureAwait(true);
				}
			}
			catch (Exception e) 
			{
				Debug.WriteLine("Exception when invoking an event asynchronously:");
				Debug.WriteLine (e.ToString ());
				throw;
			}
		}

		public void Invoke (object sender, TEventArgs args)
		{
			try
			{
				if (args != null)
					args.TimesInvoked = Repeat.Do(_id.ToString());
				foreach (var target in _invocationList) 
				{
					if (target.BlockingEvent != null) target.BlockingEvent(sender, args);
					else Task.Run(async () =>  await target.Event(sender, args).ConfigureAwait(true)).Wait();
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
			private Delegate _origObject;

			public Callback(Func<object, TEventArgs, Task> callback)
			{
				_origObject = callback;
				Event = callback;
			}

			public Callback(Action<object, TEventArgs> callback)
			{
				_origObject = callback;
				Event = convert(callback);
				BlockingEvent = callback;
			}

			public Callback(Predicate<TEventArgs> condition, TaskCompletionSource<object> tcs)
			{
				_origObject = condition;
				Event = convert(condition, tcs);
			}

			public Func<object, TEventArgs, Task> Event { get; private set; }
			public Action<object, TEventArgs> BlockingEvent { get; private set; }

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

			private Func<object, TEventArgs, Task> convert(Action<object, TEventArgs> callback)
			{
				return (sender, e) => 
				{
					callback (sender, e);
					return Task.FromResult (true);
				};
			}

			private Func<object, TEventArgs, Task> convert(Predicate<TEventArgs> condition, TaskCompletionSource<object> tcs)
			{
				return (sender, e) => 
				{
					if (!condition(e)) return Task.FromResult(true);
					tcs.TrySetResult(null);
					return Task.FromResult(true);
				};
			}
		}
	}
}

