using System.Collections.Generic;

namespace AGS.API
{
	public interface ICustomProperties : IComponent
	{
		int GetInt(string name, int defaultValue = 0);
		void SetInt(string name, int value);

		float GetFloat(string name, float defaultValue = 0f);
		void SetFloat(string name, float value);

		string GetString(string name, string defaultValue = null);
		void SetString(string name, string value);

		bool GetBool(string name, bool defaultValue = false);
		void SetBool(string name, bool value);

		IDictionary<string, int> AllInts();
		IDictionary<string, float> AllFloats();
		IDictionary<string, string> AllStrings();
		IDictionary<string, bool> AllBooleans();

		void RegisterCustomData(ICustomSerializable customData);
		void CopyFrom(ICustomProperties properties);
	}
}

