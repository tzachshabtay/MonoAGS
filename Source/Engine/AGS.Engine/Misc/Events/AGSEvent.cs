using System;
using AGS.API;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Reflection;

namespace AGS.Engine
{
    public class AGSEvent : IEvent, IBlockingEvent
	{
        private readonly EventCallbacksCollection<Callback> _invocationList;

		public AGSEvent()
		{
            _invocationList = new EventCallbacksCollection<Callback>();
		}

        #region IEvent implementation

        public int SubscribersCount => _invocationList.Count;

        public void Subscribe(Action callback, CallbackPriority priority = CallbackPriority.Normal) => subscribe(new Callback(callback), priority);

        public void Unsubscribe(Action callback, CallbackPriority priority = CallbackPriority.Normal) => unsubscribe(new Callback(callback), priority);

        public void Subscribe(ClaimableCallback callback, CallbackPriority priority = CallbackPriority.Normal) => subscribe(new Callback(callback), priority);

        public void Unsubscribe(ClaimableCallback callback, CallbackPriority priority = CallbackPriority.Normal) => unsubscribe(new Callback(callback), priority);

        public void SubscribeToAsync(Func<Task> callback, CallbackPriority priority = CallbackPriority.Normal) => subscribe(new Callback(callback), priority);

        public void UnsubscribeToAsync(Func<Task> callback, CallbackPriority priority = CallbackPriority.Normal) => unsubscribe(new Callback(callback), priority);

        public async Task WaitUntilAsync(Func<bool> condition, CallbackPriority priority = CallbackPriority.Normal)
		{
			TaskCompletionSource<object> tcs = new TaskCompletionSource<object>(null);
			var callback = new Callback(condition, tcs);
            subscribe(callback, priority);
			await tcs.Task;
            unsubscribe(callback, priority);
		}

		public async Task InvokeAsync()
		{
			try
			{
                ClaimEventToken token = new ClaimEventToken();
				foreach (var target in _invocationList)
				{
                    if (target.BlockingEventWithClaimToken != null)
                    {
                        target.BlockingEventWithClaimToken(ref token);
                        if (token.Claimed) return;
                    }
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
                ClaimEventToken token = new ClaimEventToken();
				foreach (var target in _invocationList)
				{
                    if (target.BlockingEventWithClaimToken != null)
                    {
                        target.BlockingEventWithClaimToken(ref token);
                        if (token.Claimed) return;
                    }
                    if (target.BlockingEvent != null) target.BlockingEvent();
                    else target.Event();
				}
			}
			catch (Exception e)
			{
				Debug.WriteLine("Exception when invoking an event:");
				Debug.WriteLine(e.ToString());
				throw;
			}
		}

        public void Dispose() => _invocationList?.Clear();

		#endregion

        private void subscribe(Callback callback, CallbackPriority priority)
		{
            _invocationList.Add(callback, priority);
		}

        private void unsubscribe(Callback callback, CallbackPriority priority)
		{
            _invocationList.Remove(callback, priority);
		}

		private struct Callback
		{
			private readonly Delegate _origObject;
            private string _methodName;

			public Callback(Func<Task> callback)
			{
				_origObject = callback;
				Event = callback;
                BlockingEventWithClaimToken = null;
                BlockingEvent = null;
                _methodName = null;
			}

			public Callback(Action callback)
			{
				_origObject = callback;
				Event = convert(callback);
                BlockingEvent = callback;
                BlockingEventWithClaimToken = null;
                _methodName = null;
			}

			public Callback(Func<bool> condition, TaskCompletionSource<object> tcs)
			{
				_origObject = condition;
				Event = convert(condition, tcs);
                BlockingEventWithClaimToken = null;
                BlockingEvent = null;
                _methodName = null;
			}

            public Callback(ClaimableCallback callback)
            {
                _origObject = callback;
                BlockingEventWithClaimToken = callback;
                Event = null;
                BlockingEvent = null;
                _methodName = null;
            }

			public Func<Task> Event { get; }
			public Action BlockingEvent { get; }
            public ClaimableCallback BlockingEventWithClaimToken { get; }

			public override bool Equals(object obj)
			{
                if (obj is Callback other)
                {
    				if (other._origObject == _origObject) return true;
    				if (_origObject.Target != other._origObject.Target) return false;
    				return getMethodName() == other.getMethodName();
                }
                else return false;
			}

			public override int GetHashCode()
			{
				if (_origObject.Target == null) return getMethodName().GetHashCode(); //static method subscriptions
				return _origObject.Target.GetHashCode();
			}

			public override string ToString()
			{
				return $"[Event on {_origObject.Target} ({getMethodName()})]";
			}

            private string getMethodName()
            {
                if (_methodName == null)
                {
                    _methodName = RuntimeReflectionExtensions.GetMethodInfo(_origObject).Name;
                }
                return _methodName;
            }

            private static Func<Task> convert(Action callback)
			{
				return () =>
				{
					callback();
					return Task.CompletedTask;
				};
			}

			private static Func<Task> convert(Func<bool> condition, TaskCompletionSource<object> tcs)
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
