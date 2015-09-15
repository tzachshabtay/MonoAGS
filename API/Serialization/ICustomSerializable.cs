using System;

namespace AGS.API
{
	public interface ICustomSerializable
	{
		void Load(string contents);
		ICustomSerializableData Save();
	}
}

