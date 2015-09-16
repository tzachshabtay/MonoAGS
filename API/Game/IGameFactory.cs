using System;
using System.Drawing;

namespace AGS.API
{
	public interface IGameFactory
	{
		IGraphicsFactory Graphics { get; }
		ISoundFactory Sound { get; }

		int GetInt(string name, int defaultValue = 0);
		float GetFloat(string name, float defaultValue = 0f);
		string GetString(string name, string defaultValue = null);

		ILabel GetLabel(string text, float width, float height, float x, float y);
		IObject GetObject();
		ICharacter GetCharacter();

		void RegisterCustomData(ICustomSerializable customData);
	}
}

