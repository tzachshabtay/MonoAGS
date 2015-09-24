using System;
using AGS.API;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Diagnostics;

namespace AGS.Engine
{
	public class AGSEvent<TEventArgs> : IEvent<TEventArgs> where TEventArgs : AGSEventArgs
	{
		Guid _id;

		public AGSEvent ()
		{
			_id = Guid.NewGuid();
		}

		#region IEvent implementation

		public void Subscribe (Action<object, TEventArgs> callback)
		{
			invocationList.Add (convert (callback));
		}

		public void Unsubscribe (Action<object, TEventArgs> callback)
		{
			invocationList.Remove (convert (callback));
		}

		public void SubscribeToAsync (Func<object, TEventArgs, Task> callback)
		{
			invocationList.Add (callback);
		}

		public void UnsubscribeToAsync (Func<object, TEventArgs, Task> callback)
		{
			invocationList.Remove (callback);
		}

		public async Task InvokeAsync (object sender, TEventArgs args)
		{
			try
			{
				if (args != null)
					args.TimesInvoked = Repeat.Do(_id.ToString());
				foreach (var target in invocationList) 
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

		List<Func<object, TEventArgs, Task>> invocationList = new List<Func<object, TEventArgs, Task>>(); 
	}
}

