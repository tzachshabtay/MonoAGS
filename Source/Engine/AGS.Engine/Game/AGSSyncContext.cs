using System;
using System.Collections.Concurrent;
using System.Threading;

namespace AGS.Engine
{
	public abstract class AGSSyncContext : SynchronizationContext, IMessagePump, IThreadInvoke
	{
		private ConcurrentQueue<Action> _queue;

		public AGSSyncContext ()
		{
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

    public class RenderThreadSyncContext : AGSSyncContext, IRenderMessagePump, IRenderThread
    {
        private static bool _initialized;

        public RenderThreadSyncContext()
        {
            if (_initialized) throw new InvalidOperationException("Render Synchronization context was already initialized!");
            _initialized = true;
        }
    }

    public class UpdateThreadSyncContext : AGSSyncContext, IUpdateMessagePump, IUpdateThread
    {
        private static bool _initialized;

        public UpdateThreadSyncContext()
        {
            if (_initialized) throw new InvalidOperationException("Update Synchronization context was already initialized!");
            _initialized = true;
        }
    }
}

