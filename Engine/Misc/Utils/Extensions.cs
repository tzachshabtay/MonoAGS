using System;
using System.Collections.Generic;

namespace Engine
{
	public static class Extensions
	{
		public static TValue GetOrAdd<TKey, TValue>(this Dictionary<TKey, TValue> map, TKey key, Func<TValue> getValue)
		{
			TValue value;
			if (!map.TryGetValue (key, out value)) 
			{
				value = getValue ();
				map.Add (key, value);
			}
			return value;
		}
	}
}

