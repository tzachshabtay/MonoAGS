using System;

namespace API
{
	public interface IGameFactory
	{
		IGraphicsFactory Graphics { get; }
		ISoundFactory Sound { get; }

		int GetInt(string name, int defaultValue = 0);
		float GetFloat(string name, float defaultValue = 0f);
		string GetString(string name, string defaultValue = null);

		void RegisterCustomData(ICustomSerializable customData);
	}
}

