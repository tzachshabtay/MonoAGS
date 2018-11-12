using System;
using System.Diagnostics;
using System.Reflection;
using System.Threading.Tasks;
using AGS.API;

namespace AGS.Engine
{
    public class AGSEvent<TEventArgs> : IEvent<TEventArgs>, IBlockingEvent<TEventArgs>
    {
        private readonly EventCallbacksCollection<Callback> _invocationList;

        public AGSEvent()
        {
            _invocationList = new EventCallbacksCollection<Callback>();
        }

        #region IEvent implementation

        public int SubscribersCount => _invocationList.Count;

        public void Subscribe(Action<TEventArgs> callback, CallbackPriority priority = CallbackPriority.Normal) => subscribe(new Callback(callback), priority);

        public void Subscribe(Action callback, CallbackPriority priority = CallbackPriority.Normal) => subscribe(new Callback(callback), priority);

        public void Subscribe(ClaimableCallbackWithArgs<TEventArgs> callback, CallbackPriority priority = CallbackPriority.Normal) => subscribe(new Callback(callback), priority);

        public void Unsubscribe(Action<TEventArgs> callback, CallbackPriority priority = CallbackPriority.Normal) => unsubscribe(new Callback(callback), priority);

        public void Unsubscribe(Action callback, CallbackPriority priority = CallbackPriority.Normal) => unsubscribe(new Callback(callback), priority);

        public void Unsubscribe(ClaimableCallbackWithArgs<TEventArgs> callback, CallbackPriority priority = CallbackPriority.Normal) => unsubscribe(new Callback(callback), priority);

        public void SubscribeToAsync (Func<TEventArgs, Task> callback, CallbackPriority priority = CallbackPriority.Normal) => subscribe(new Callback (callback), priority);

        public void UnsubscribeToAsync (Func<TEventArgs, Task> callback, CallbackPriority priority = CallbackPriority.Normal) => unsubscribe(new Callback (callback), priority);

        public void SubscribeToAsync(Func<Task> callback, CallbackPriority priority = CallbackPriority.Normal) => subscribe(new Callback(callback), priority);

        public void UnsubscribeToAsync(Func<Task> callback, CallbackPriority priority = CallbackPriority.Normal) => unsubscribe(new Callback(callback), priority);

        public async Task WaitUntilAsync(Predicate<TEventArgs> condition, CallbackPriority priority = CallbackPriority.Normal)
        {
            TaskCompletionSource<object> tcs = new TaskCompletionSource<object> (null);
            var callback = new Callback(condition, tcs);
            subscribe(callback, priority);
            await tcs.Task;
            unsubscribe(callback, priority);
        }

        public async Task InvokeAsync (TEventArgs args)
        {
            try
            {
                ClaimEventToken token = new ClaimEventToken();
                foreach (var target in _invocationList) 
                {
                    if (target.BlockingEvent != null) target.BlockingEvent(args);
                    else if (target.BlockingEventWithToken != null)
                    {
                        target.BlockingEventWithToken(args, ref token);
                        if (token.Claimed) return;
                    }
                    else if (target.EmptyBlockingEvent != null) target.EmptyBlockingEvent();
                    else if (target.EmptyEvent != null) await target.EmptyEvent();
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
                ClaimEventToken token = new ClaimEventToken();
                foreach (var target in _invocationList) 
                {
                    if (target.BlockingEvent != null) target.BlockingEvent(args);
                    else if (target.BlockingEventWithToken != null)
                    {
                        target.BlockingEventWithToken(args, ref token);
                        if (token.Claimed) return;
                    }
                    else if (target.EmptyBlockingEvent != null) target.EmptyBlockingEvent();
                    else if (target.EmptyEvent != null) target.EmptyEvent();
                    else target.Event(args);
                }
            }
            catch (Exception e) 
            {
                Debug.WriteLine("Exception when invoking an event:");
                Debug.WriteLine (e.ToString ());
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

        private class Callback
        {
            private readonly Delegate _origObject;
            private string _methodName;

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

            public Callback(Action callback)
            {
                _origObject = callback;
                EmptyBlockingEvent = callback;
            }

            public Callback(Func<Task> callback)
            {
                _origObject = callback;
                EmptyEvent = callback;
            }

            public Callback(ClaimableCallbackWithArgs<TEventArgs> callback)
            {
                _origObject = callback;
                BlockingEventWithToken = callback;
            }

            public Func<TEventArgs, Task> Event { get; }
            public Action<TEventArgs> BlockingEvent { get; }
            public ClaimableCallbackWithArgs<TEventArgs> BlockingEventWithToken { get; }
            public Func<Task> EmptyEvent { get; }
            public Action EmptyBlockingEvent { get; }

            public override bool Equals(object obj)
            {
                Callback other = obj as Callback;
                if (other == null) return false;
                if (other._origObject == _origObject) return true;
                if (_origObject.Target != other._origObject.Target) return false;
                return getMethodName() == other.getMethodName();
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
}
