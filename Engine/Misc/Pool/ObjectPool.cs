using System;
using System.Collections.Concurrent;

namespace AGS.Engine
{
	public class ObjectPool<TPoolItem>
	{
		private readonly ConcurrentQueue<TPoolItem> _queue;
		private readonly Func<TPoolItem> _factory;
		private readonly Action<TPoolItem> _release;

		public ObjectPool(Func<TPoolItem> factory, int initialCount, Action<TPoolItem> release = null)
		{
			_queue = new ConcurrentQueue<TPoolItem> ();
			_factory = factory;
			_release = release;
			for (int i = 0; i < initialCount; i++) _queue.Enqueue(_factory());
		}

		public TPoolItem Acquire()
		{
			TPoolItem item;
			if (_queue.TryDequeue(out item)) return item;
			return _factory ();
		}

		public void Release(TPoolItem item)
		{
			if (_release != null) _release(item);
			_queue.Enqueue(item);
		}
	}
}

