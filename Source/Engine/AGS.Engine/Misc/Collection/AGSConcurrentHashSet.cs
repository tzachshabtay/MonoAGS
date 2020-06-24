﻿using System;
using AGS.API;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections;

namespace AGS.Engine
{
	public class AGSConcurrentHashSet<TItem> : IConcurrentHashSet<TItem>
	{
		//We're using a concurrent dictionary with a value we don't care about to simulate a hash set.
		private ConcurrentDictionary<TItem, byte> _map;

		public AGSConcurrentHashSet(int capacity = 5, bool fireListChangedEvent = true)
		{
			_map = new ConcurrentDictionary<TItem, byte> (2, capacity);
            if (fireListChangedEvent)
                OnListChanged = new AGSEvent<AGSHashSetChangedEventArgs<TItem>>();
		}

        #region IConcurrentCollection implementation

        public int Count => _map.Count;

        public IBlockingEvent<AGSHashSetChangedEventArgs<TItem>> OnListChanged { get; }

        private void onListChanged(TItem item, ListChangeType changeType)
        {
            var onListChanged = OnListChanged;
            if (onListChanged == null) return;
            onListChanged.Invoke(new AGSHashSetChangedEventArgs<TItem>(changeType, item));
        } 

        private void onListChanged(List<TItem> items, ListChangeType changeType)
        {
            var onListChanged = OnListChanged;
            if (onListChanged == null) return;
            onListChanged.Invoke(new AGSHashSetChangedEventArgs<TItem>(changeType, items));
        }

		public bool Add(TItem item)
		{
			bool added = _map.TryAdd(item, 0);
            if (added) onListChanged(item, ListChangeType.Add);
            return added;
		}

        public int AddRange(List<TItem> items)
        {
            List<TItem> addedItems = new List<TItem>(items.Count);
            foreach (var item in items)
            {
                if (_map.TryAdd(item, 0))
                {
                    addedItems.Add(item);
                }
            }
            if (addedItems.Count > 0) onListChanged(addedItems, ListChangeType.Add);
            return addedItems.Count;
        }

		public bool Remove(TItem item)
		{
		    bool removed = _map.TryRemove(item, out byte _);
            if (removed) onListChanged(item, ListChangeType.Remove);
            return removed;
		}

		public void RemoveAll(Predicate<TItem> shouldRemove)
		{
			foreach (var item in _map.Keys)
			{
				if (shouldRemove(item)) Remove(item);
			}
		}

        public bool Contains(TItem item) => _map.ContainsKey(item);

        public void Clear()
		{
            RemoveAll(_ => true);
		}

		#endregion

		#region IEnumerable implementation

		public IEnumerator<TItem> GetEnumerator()
		{
            var enumerator =  _map.GetEnumerator();
            try
            {
                while (enumerator.MoveNext())
                {
                    yield return enumerator.Current.Key;
                }
            }
            finally
            {
                enumerator.Dispose();
            }
		}

		#endregion

		#region IEnumerable implementation

		IEnumerator IEnumerable.GetEnumerator()
		{
			return ((IEnumerable)_map.Keys).GetEnumerator();
		}

		#endregion
	}
}

