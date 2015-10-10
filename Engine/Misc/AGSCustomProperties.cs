using System;
using AGS.API;
using System.Collections.Generic;

namespace AGS.Engine
{
	public class AGSCustomProperties : ICustomProperties
	{
		private Dictionary<string, int> ints;
		private Dictionary<string, float> floats;
		private Dictionary<string, string> strings;

		public AGSCustomProperties()
		{
			ints = new Dictionary<string, int> ();
			floats = new Dictionary<string, float> ();
			strings = new Dictionary<string, string> ();
		}

		#region ICustomProperties implementation

		public int GetInt(string name, int defaultValue = 0)
		{
			return ints.GetOrAdd(name, () => defaultValue);
		}

		public void SetInt(string name, int value)
		{
			ints[name] = value;
		}

		public float GetFloat(string name, float defaultValue = 0f)
		{
			return floats.GetOrAdd(name, () => defaultValue);
		}

		public void SetFloat(string name, float value)
		{
			floats[name] = value;
		}

		public string GetString(string name, string defaultValue = null)
		{
			return strings.GetOrAdd(name, () => defaultValue);
		}

		public void SetString(string name, string value)
		{
			strings[name] = value;
		}

		public void RegisterCustomData(ICustomSerializable customData)
		{
			throw new NotImplementedException();
		}

		#endregion
	}
}

