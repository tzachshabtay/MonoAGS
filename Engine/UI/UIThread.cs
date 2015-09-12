using System;
using System.Threading;
using System.Collections.Concurrent;

namespace Engine
{
	public static class UIThread
	{
		const string threadName = "AGS UI Thread";
		private static ConcurrentQueue<Action> queue = new ConcurrentQueue<Action>();

		public static void Init()
		{
			if (Thread.CurrentThread.Name == null)
				Thread.CurrentThread.Name = threadName;
		}

		public static void Invoke(Action action)
		{
			if (Thread.CurrentThread.Name == threadName) 
			{
				action ();
				return;
			}
			ManualResetEvent resetEvent = new ManualResetEvent (false);
			Action waitAction = () =>
			{
				try
				{
					action();
				}
				finally
				{
					resetEvent.Set();
				}
			};
			queue.Enqueue (waitAction);
			resetEvent.WaitOne ();
		}

		public static void BeginInvoke(Action action)
		{
			if (Thread.CurrentThread.Name == threadName) 
			{
				action ();
				return;
			}
			queue.Enqueue (action);
		}

		public static void UITick()
		{
			Action action;
			while (queue.TryDequeue (out action))
				action ();
		}
	}
}

