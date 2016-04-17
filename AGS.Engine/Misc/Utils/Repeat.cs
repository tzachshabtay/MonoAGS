using System;
using System.Threading;
using System.Collections.Generic;
using System.Linq;

namespace AGS.Engine
{
	public static class Repeat
	{
		private static Dictionary<string, Counter> _times = new Dictionary<string, Counter>(100);

		public static bool OnceOnly(string key)
		{
			return Do(key) == 1;
		}

		public static bool Exactly(string key, int times)
		{
			return Do(key) == times;
		}

		public static bool MoreThan(string key, int times)
		{
			return Do(key) > times;
		}

		public static bool LessThan(string key, int times)
		{
			return Do(key) < times;
		}

		public static int Do(string key)
		{
			return getCounter(key).Increment();
		}

		public static int Current(string key)
		{
			return getCounter(key).Value;
		}

		public static void Reset(string key)
		{
			getCounter(key).Reset();
		}
        
        public static Dictionary<string, int> ToDictionary()
        {
            return _times.ToDictionary(k => k.Key, v => v.Value.Value);
        }
        
        public static void FromDictionary(Dictionary<string, int> counters)
        {
            _times = counters.ToDictionary(k => k.Key, v => new Counter(v.Value));
        }

		private static Counter getCounter(string key)
		{
			return _times.GetOrAdd(key, () => new Counter ());
		}
			
		private class Counter
		{
            public Counter(int value = 0)
            {
                _value = value;
            }
            
			private int _value;

			public int Value { get { return _value; }}

			public int Increment()
			{
				return Interlocked.Increment(ref _value);
			}

			public void Reset()
			{
				_value = 0;
			}
		}
	}
}

