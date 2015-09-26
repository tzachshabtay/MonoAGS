using System;
using AGS.API;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Diagnostics;

namespace AGS.Engine
{
	public class AGSEvent<TEventArgs> : IEvent<TEventArgs> where TEventArgs : AGSEventArgs
	{
		private readonly Guid _id;
		private readonly IConcurrentHashSet<Func<object, TEventArgs, Task>> _invocationList;

		public AGSEvent ()
		{
			_id = Guid.NewGuid();
			_invocationList = new AGSConcurrentHashSet<Func<object, TEventArgs, Task>> ();
		}

		#region IEvent implementation

		public void Subscribe (Action<object, TEventArgs> callback)
		{
			if (_invocationList.Count > 100)
			{
				Debug.WriteLine("Subscribe!!! " + new StackTrace ().ToString());
				return;
			}
			_invocationList.Add (convert (callback));
		}

		public void Unsubscribe (Action<object, TEventArgs> callback)
		{
			_invocationList.Remove (convert (callback));
		}

		public void WaitUntil(Predicate<TEventArgs> condition)
		{
			Task.Run(async () => await WaitUntilAsync(condition)).Wait();
		}

		public void SubscribeToAsync (Func<object, TEventArgs, Task> callback)
		{
			if (_invocationList.Count > 100)
			{
				Debug.WriteLine("Subscribe!!! " + new StackTrace ().ToString());
				return;
			}
			_invocationList.Add (callback);
		}

		public void UnsubscribeToAsync (Func<object, TEventArgs, Task> callback)
		{
			_invocationList.Remove (callback);
		}

		public async Task WaitUntilAsync(Predicate<TEventArgs> condition)
		{
			TaskCompletionSource<object> tcs = new TaskCompletionSource<object> (null);
			var callback = convert(condition, tcs);
			SubscribeToAsync(callback);
			await tcs.Task;
			UnsubscribeToAsync(callback);
		}

		public async Task InvokeAsync (object sender, TEventArgs args)
		{
			try
			{
				if (args != null)
					args.TimesInvoked = Repeat.Do(_id.ToString());
				foreach (var target in _invocationList) 
				{
					await target (sender, args).ConfigureAwait(true);
				}
			}
			catch (Exception e) 
			{
				Debug.WriteLine("Exception when invoking an event:");
				Debug.WriteLine (e.ToString ());
				throw;
			}
		}

		public void Invoke (object sender, TEventArgs args)
		{
			Task.Run (async () => await InvokeAsync (sender, args)).Wait ();
		}

		#endregion

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

