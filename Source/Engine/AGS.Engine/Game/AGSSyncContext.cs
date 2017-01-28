using System;
using System.Collections.Concurrent;
using System.Threading;

namespace AGS.Engine
{
	public class AGSSyncContext : SynchronizationContext, IMessagePump, IUIThread
	{
		private ConcurrentQueue<Action> _queue;
		private static bool _initialized;

		public AGSSyncContext ()
		{
			if (_initialized) throw new InvalidOperationException ("AGS Synchronization context was already initialized!");
			_initialized = true;
			_queue = new ConcurrentQueue<Action> ();
		}

		public void SetSyncContext ()
		{
			SynchronizationContext.SetSynchronizationContext (this);
		}

		public void PumpMessages ()
		{
			Action action;
			while (_queue.TryDequeue (out action)) {
				action ();
			}
		}

		public override void Post (SendOrPostCallback d, object state)
		{
			_queue.Enqueue (() => d (state));
		}

		public override void Send (SendOrPostCallback d, object state)
		{
			ManualResetEvent resetEvent = new ManualResetEvent (false);
			Action action = () => {
				d (state);
				resetEvent.Set ();
			};
			_queue.Enqueue (action);
            resetEvent.WaitOne(120000);
		}

        public void RunBlocking(Action action)
        { 
            if (Environment.CurrentManagedThreadId == AGSGame.UIThreadID)
            {
                action();
                return;
            }
            Send((state) => action(), null);
        }
	}
}

