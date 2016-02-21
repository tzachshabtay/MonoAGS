using System;
using AGS.API;
using System.Collections.Generic;

namespace AGS.Engine
{
	public class AGSCustomProperties : ICustomProperties
	{
		private readonly Dictionary<string, int> _ints;
		private readonly Dictionary<string, float> _floats;
		private readonly Dictionary<string, string> _strings;
		private readonly Dictionary<string, bool> _bools;

		public AGSCustomProperties()
		{
			_ints = new Dictionary<string, int> ();
			_floats = new Dictionary<string, float> ();
			_strings = new Dictionary<string, string> ();
			_bools = new Dictionary<string, bool> ();
		}

		#region ICustomProperties implementation

		public int GetInt(string name, int defaultValue = 0)
		{
			return _ints.GetOrAdd(name, () => defaultValue);
		}

		public void SetInt(string name, int value)
		{
			_ints[name] = value;
		}

		public float GetFloat(string name, float defaultValue = 0f)
		{
			return _floats.GetOrAdd(name, () => defaultValue);
		}

		public void SetFloat(string name, float value)
		{
			_floats[name] = value;
		}

		public string GetString(string name, string defaultValue = null)
		{
			return _strings.GetOrAdd(name, () => defaultValue);
		}

		public void SetString(string name, string value)
		{
			_strings[name] = value;
		}

		public bool GetBool(string name, bool defaultValue = false)
		{
			return _bools.GetOrAdd(name, () => defaultValue);
		}

		public void SetBool(string name, bool value)
		{
			_bools[name] = value;
		}

		public void RegisterCustomData(ICustomSerializable customData)
		{
			throw new NotImplementedException();
		}
			
		public IDictionary<string, int> AllInts()
		{
			return _ints;
		}

		public IDictionary<string, float> AllFloats()
		{
			return _floats;
		}

		public IDictionary<string, string> AllStrings()
		{
			return _strings;
		}

		public IDictionary<string, bool> AllBooleans()
		{
			return _bools;
		}
			
		public void CopyFrom(ICustomProperties properties)
		{
			if (properties == null) return;
			addMap(AllInts(), properties.AllInts());
			addMap(AllBooleans(), properties.AllBooleans());
			addMap(AllStrings(), properties.AllStrings());
			addMap(AllFloats(), properties.AllFloats());
		}

		private void addMap<TValue>(IDictionary<string, TValue> appendTo, IDictionary<string, TValue> appendFrom)
		{
			foreach (var pair in appendFrom)
			{
				appendTo[pair.Key] = pair.Value;
			}
		}
		#endregion
	}
}

