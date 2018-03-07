using System;
using System.Collections;
using System.Collections.Generic;
using AGS.API;

namespace AGS.Engine
{
    public class EventCallbacksSingleList<Callback> : IEnumerable<Callback>
    {
        private const int MAX_SUBSCRIPTIONS = 10000;

        private readonly IConcurrentHashSet<Callback> _list;

        public EventCallbacksSingleList()
        {
            _list = new AGSConcurrentHashSet<Callback>(fireListChangedEvent: false); 
        }

        public int Count { get; private set; }

        public void Add(Callback callback)
        {
            if (!_list.Add(callback)) return;
            Count++;
            if (Count > MAX_SUBSCRIPTIONS)
            {
                throw new OverflowException("Event Subscription Overflow");
            }
        }

        public void Remove(Callback callback)
        {
            if (!_list.Remove(callback)) return;
            Count--;
        }

        public IEnumerator<Callback> GetEnumerator()
        {
            return _list.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
