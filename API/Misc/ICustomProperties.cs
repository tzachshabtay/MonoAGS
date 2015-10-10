using System;

namespace AGS.API
{
	public interface ICustomProperties
	{
		int GetInt(string name, int defaultValue = 0);
		void SetInt(string name, int value);

		float GetFloat(string name, float defaultValue = 0f);
		void SetFloat(string name, float value);

		string GetString(string name, string defaultValue = null);
		void SetString(string name, string value);

		void RegisterCustomData(ICustomSerializable customData);
	}
}

