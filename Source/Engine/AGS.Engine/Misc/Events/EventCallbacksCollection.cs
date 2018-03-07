using System;
using System.Collections;
using System.Collections.Generic;
using AGS.API;

namespace AGS.Engine
{
    public class EventCallbacksCollection<Callback> : IEnumerable<Callback>
    {
        private readonly EventCallbacksSingleList<Callback> _high, _normal, _low;

        public EventCallbacksCollection()
        {
            _high = new EventCallbacksSingleList<Callback>();
            _normal = new EventCallbacksSingleList<Callback>();
            _low = new EventCallbacksSingleList<Callback>();
        }

        public int Count => _high.Count + _normal.Count + _low.Count;

        public void Add(Callback callback, CallbackPriority priority)
        {
            switch (priority)
            {
                case CallbackPriority.High:
                    _high.Add(callback);
                    break;
                case CallbackPriority.Normal:
                    _normal.Add(callback);
                    break;
                case CallbackPriority.Low:
                    _low.Add(callback);
                    break;
                default:
                    throw new NotSupportedException($"Unsupported callback priority: ${priority}");
            }
        }

        public void Remove(Callback callback, CallbackPriority priority)
        {
            switch (priority)
            {
                case CallbackPriority.High:
                    _high.Remove(callback);
                    break;
                case CallbackPriority.Normal:
                    _normal.Remove(callback);
                    break;
                case CallbackPriority.Low:
                    _low.Remove(callback);
                    break;
                default:
                    throw new NotSupportedException($"Unsupported callback priority: ${priority}");
            }
        }

        public IEnumerator<Callback> GetEnumerator()
        {
            foreach (var callback in _high) yield return callback;
            foreach (var callback in _normal) yield return callback;
            foreach (var callback in _low) yield return callback;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
