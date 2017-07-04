using System;
using System.Collections.Generic;
using System.Collections;
using AGS.API;
using System.Diagnostics;

namespace AGS.Engine
{
	public class AGSBindingList<TItem> : IAGSBindingList<TItem>
	{
		private List<TItem> _list;

		public AGSBindingList(int capacity)
		{
			_list = new List<TItem>(capacity);
			OnListChanged = new AGSEvent<AGSListChangedEventArgs<TItem>> ();
		}

		public AGSBindingList(IEnumerable<TItem> collection)
		{
			_list = new List<TItem> (collection);
		}

		public IEvent<AGSListChangedEventArgs<TItem>> OnListChanged { get; private set; }

		private void onListChanged(TItem item, int index, ListChangeType changeType)
		{
            OnListChanged.Invoke(new AGSListChangedEventArgs<TItem>(changeType, new AGSListItem<TItem>(item, index)));
		}

        private void onListChanged(IEnumerable<AGSListItem<TItem>> items, ListChangeType changeType)
        {
	        OnListChanged.Invoke(new AGSListChangedEventArgs<TItem>(changeType, items));
        }

		#region IList implementation

		public int IndexOf(TItem item)
		{
			return _list.IndexOf(item);
		}

		public void Insert(int index, TItem item)
		{
			_list.Insert(index, item);
			onListChanged(item, index, ListChangeType.Add);
		}

		public void RemoveAt(int index)
		{
			var item = _list[index];
			_list.RemoveAt(index);
			onListChanged(item, index, ListChangeType.Remove);
		}

		public TItem this[int index]
		{
			get
			{
				return _list[index];
			}
			set
			{
				var item = _list[index];
				_list[index] = value;
				onListChanged(item, index, ListChangeType.Remove);
				onListChanged(value, index, ListChangeType.Add);
			}
		}

        #endregion

        public void AddRange(List<TItem> items)
        {
            List<AGSListItem<TItem>> pairs = new List<AGSListItem<TItem>>(items.Count);
            int index = _list.Count;
            foreach (var item in items)
            {
                _list.Add(item);
                pairs.Add(new AGSListItem<TItem>(item, index));
                index++;
            }
            onListChanged(pairs, ListChangeType.Add);
        }

		#region ICollection implementation

		public void Add(TItem item)
		{
			_list.Add(item);
			onListChanged(item, _list.Count - 1, ListChangeType.Add);
		}

		public void Clear()
		{
			for (int i = _list.Count - 1; i >= 0; i--)
			{
				RemoveAt(i);
			}
		}

		public bool Contains(TItem item)
		{
			return _list.Contains(item);
		}

		public void CopyTo(TItem[] array, int arrayIndex)
		{
			_list.CopyTo(array, arrayIndex);
		}

		public bool Remove(TItem item)
		{
			int index = IndexOf(item);
            if (index < 0) return false;
            try
            {
                _list.RemoveAt(index);
            }
            catch (ArgumentOutOfRangeException e)
            {
                if (Repeat.LessThan("AGSBindingList.Remove Exception", 5))
                {
                    Debug.WriteLine("Tried to remove an already removed item from the binding list. Item: {0}, Exception: {1}", 
                                    item, e);
                }
                return false;
            }
			onListChanged(item, index, ListChangeType.Remove);
			return true;
		}

		public int Count
		{
			get
			{
				return _list.Count;
			}
		}

		public bool IsReadOnly
		{
			get
			{
				return ((ICollection<TItem>)_list).IsReadOnly;
			}
		}

		#endregion

		#region IEnumerable implementation

		public IEnumerator<TItem> GetEnumerator()
		{
			return _list.GetEnumerator();
		}

		#endregion

		#region IEnumerable implementation

		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return ((IEnumerable)_list).GetEnumerator();
		}

		#endregion
	}
}

