using System;
using System.Diagnostics;
using System.Reflection;
using System.Threading.Tasks;
using AGS.API;

namespace AGS.Engine
{
    public class AGSEvent<TEventArgs> : IEvent<TEventArgs>, IBlockingEvent<TEventArgs>
    {
        private readonly IConcurrentHashSet<Callback> _invocationList;
        private readonly EventSubscriberCounter _counter = new EventSubscriberCounter();

        public AGSEvent()
        {
            _invocationList = new AGSConcurrentHashSet<Callback>(fireListChangedEvent: false);
        }

        #region IEvent implementation

        public int SubscribersCount => _counter.Count;

        public void Subscribe(Action<TEventArgs> callback) => subscribe(new Callback(callback));

        public void Subscribe(Action callback) => subscribe(new Callback(callback));

        public void Subscribe(ClaimableCallbackWithArgs<TEventArgs> callback) => subscribe(new Callback(callback));

        public void Unsubscribe(Action<TEventArgs> callback) => unsubscribe(new Callback(callback));

        public void Unsubscribe(Action callback) => unsubscribe(new Callback(callback));

        public void Unsubscribe(ClaimableCallbackWithArgs<TEventArgs> callback) => unsubscribe(new Callback(callback));

        public void SubscribeToAsync (Func<TEventArgs, Task> callback) => subscribe(new Callback (callback));

        public void UnsubscribeToAsync (Func<TEventArgs, Task> callback) => unsubscribe(new Callback (callback));

        public void SubscribeToAsync(Func<Task> callback) => subscribe(new Callback(callback));

        public void UnsubscribeToAsync(Func<Task> callback) => unsubscribe(new Callback(callback));

        public async Task WaitUntilAsync(Predicate<TEventArgs> condition)
        {
            TaskCompletionSource<object> tcs = new TaskCompletionSource<object> (null);
            var callback = new Callback(condition, tcs);
            subscribe(callback);
            await tcs.Task;
            unsubscribe(callback);
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

        #endregion

        private void subscribe(Callback callback)
        {
            if (_invocationList.Add (callback))
            {
                _counter.Add();
            }
        }

        private void unsubscribe(Callback callback)
        {
            if (_invocationList.Remove(callback))
            {
                _counter.Remove();
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
                return getMethodName(_origObject) == getMethodName(other._origObject);
            }

            public override int GetHashCode()
            {
                if (_origObject.Target == null) return getMethodName(_origObject).GetHashCode(); //static method subscriptions
                return _origObject.Target.GetHashCode();
            }

            public override string ToString()
            {
                return $"[Event on {_origObject.Target.ToString()} ({getMethodName(_origObject)})]";
            }

            private string getMethodName(Delegate del) => RuntimeReflectionExtensions.GetMethodInfo(del).Name;

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